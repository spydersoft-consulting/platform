using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Spydersoft.Platform.Exceptions;
using Spydersoft.Platform.Hosting.Options;
using Spydersoft.Platform.Hosting.Telemetry;
using Spydersoft.Platform.Telemetry;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace Spydersoft.Platform.Hosting.StartupExtensions;

/// <summary>
/// Extension methods for configuring OpenTelemetry and Serilog in ASP.NET Core applications.
/// </summary>
public static class TelemetryExtensions
{
    #region Public Startup Extensions    
    /// <summary>
    /// Adds Serilog as a console logger, plus any other sinks configured in appsettings.
    /// </summary>
    /// <remarks>
    /// When using this in conjunction with OpenTelemetry, make sure <paramref name="writeToProviders"/> is set to <c>true</c>.  Additionally, the
    /// Serilog configuration for log levels overrides the any levels in the Logging section.
    /// </remarks>
    /// <param name="appBuilder">The application builder.</param>
    /// <param name="writeToProviders">if set to <c>true</c> [write to providers].</param>
    public static void AddSpydersoftSerilog(this WebApplicationBuilder appBuilder, bool writeToProviders = false)
    {
        appBuilder.Services.AddSerilog(config =>
        {
            config
            .ReadFrom.Configuration(appBuilder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);
        }, writeToProviders: writeToProviders);
    }

    /// <summary>
    /// Adds OpenTelemetry tracing, metrics, and logging to the application with advanced configuration options.
    /// </summary>
    /// <param name="appBuilder">The web application builder.</param>
    /// <param name="startupAssembly">The assembly used to determine the service version.</param>
    /// <param name="configurationFunctions">Optional configuration functions for customizing telemetry behavior.</param>
    /// <returns>The web application builder for method chaining.</returns>
    public static WebApplicationBuilder AddSpydersoftTelemetry(this WebApplicationBuilder appBuilder,
        Assembly startupAssembly,
        ConfigurationFunctions? configurationFunctions)
    {
        var telemetryOptions = new TelemetryOptions();
        appBuilder.Configuration.GetSection(TelemetryOptions.SectionName).Bind(telemetryOptions);

        // Add TelemetryOptions to the service collection for the Healthcheck
        appBuilder.Services.Configure<TelemetryOptions>(appBuilder.Configuration.GetSection(TelemetryOptions.SectionName));

        if (!telemetryOptions.Enabled)
        {
            // Register NullTelemetryClient when telemetry is disabled
            appBuilder.Services.AddSingleton<ITelemetryClient>(NullTelemetryClient.Instance);
            return appBuilder;
        }

        var version = startupAssembly.GetName().Version?.ToString() ?? "unknown";

        // Register Meter as a singleton
        appBuilder.Services.AddSingleton(sp => new Meter(telemetryOptions.MeterName, version));

        // Register ActivitySource as a singleton
        appBuilder.Services.AddSingleton(sp => new ActivitySource(telemetryOptions.ActivitySourceName, version));

        // Register ITelemetryClient using MeterTelemetryClient
        appBuilder.Services.AddSingleton<ITelemetryClient>(sp =>
        {
            var meter = sp.GetRequiredService<Meter>();
            var activitySource = sp.GetRequiredService<ActivitySource>();
            return new MeterTelemetryClient(meter, activitySource);
        });

        // Use IConfiguration binding for AspNetCore instrumentation options.
        appBuilder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(appBuilder.Configuration.GetSection(telemetryOptions.AspNetCoreInstrumentationSection));

        appBuilder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(
                    serviceName: telemetryOptions.ServiceName,
                    serviceVersion: startupAssembly.GetName().Version?.ToString() ?? "unknown",
                    serviceInstanceId: Environment.MachineName))
            .WithTracing(builder => ConfigureTracing(builder, appBuilder.Configuration, telemetryOptions, configurationFunctions))
            .WithMetrics(builder => ConfigureMetrics(builder, appBuilder.Configuration, telemetryOptions, configurationFunctions))
            .WithLogging(builder => ConfigureLogging(builder, appBuilder.Configuration, telemetryOptions, configurationFunctions));

        return appBuilder;
    }

    /// <summary>
    /// Adds OpenTelemetry tracing, metrics, and logging to the application with default configuration.
    /// </summary>
    /// <param name="appBuilder">The web application builder.</param>
    /// <param name="startupAssembly">The assembly used to determine the service version.</param>
    /// <returns>The web application builder for method chaining.</returns>
    public static WebApplicationBuilder AddSpydersoftTelemetry(this WebApplicationBuilder appBuilder, Assembly startupAssembly)
    {
        return AddSpydersoftTelemetry(appBuilder, startupAssembly, null);
    }

    #endregion


    #region Private Startup Helpers
    private static void ConfigureTracing(TracerProviderBuilder builder, ConfigurationManager configuration, TelemetryOptions options, ConfigurationFunctions? configFunctions)
    {
        // Ensure the TracerProvider subscribes to any custom ActivitySources.
        builder
            .AddSource(options.ActivitySourceName)
            .SetSampler(new AlwaysOnSampler())
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation((options) =>
            {
                options.Filter = configFunctions?.AspNetFilterFunction;
                options.EnrichWithException = configFunctions?.AspNetExceptionEnrichAction;
                options.EnrichWithHttpRequest = configFunctions?.AspNetRequestEnrichAction;
                options.EnrichWithHttpResponse = configFunctions?.AspNetResponseEnrichAction;
            });

        switch (options.Trace.Type)
        {
            case "zipkin":
                builder.AddZipkinExporter();

                builder.ConfigureServices(services =>
                {
                    // Use IConfiguration binding for Zipkin exporter options.
                    services.Configure<ZipkinExporterOptions>(configuration.GetSection(options.Trace.ZipkinConfigurationSection));
                });
                break;

            case "otlp":
                builder.AddOtlpExporter(otlpOptions => SetOltpOptions(configuration, otlpOptions, options.Trace.Otlp));
                break;

            case "console":
                builder.AddConsoleExporter();
                break;
            case "none":
            default:
                builder.AddConsoleExporter();
                break;
        }

        var cacheOptions = new FusionCacheConfigOptions();
        configuration.GetSection(FusionCacheConfigOptions.SectionName).Bind(cacheOptions);

        if (cacheOptions.Enabled)
        {
            builder.AddFusionCacheInstrumentation();
        }

        configFunctions?.TraceConfiguration?.Invoke(builder);
    }

    private static void ConfigureMetrics(MeterProviderBuilder builder, ConfigurationManager configuration, TelemetryOptions options, ConfigurationFunctions? configFunctions)
    {
        builder
            .AddMeter(options.MeterName)
            .SetExemplarFilter(ExemplarFilterType.TraceBased)
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation();

        switch (options.Metrics.HistogramAggregation)
        {
            case "exponential":
                builder.AddView(instrument =>
                {
                    return instrument.GetType().GetGenericTypeDefinition() == typeof(Histogram<>)
                        ? new Base2ExponentialBucketHistogramConfiguration()
                        : null;
                });
                break;
            default:
                // Explicit bounds histogram is the default.
                // No additional configuration necessary.
                break;
        }

        switch (options.Metrics.Type)
        {
            case "prometheus":
                builder.AddPrometheusExporter();
                break;
            case "otlp":
                builder.AddOtlpExporter(otlpOptions => SetOltpOptions(configuration, otlpOptions, options.Metrics.Otlp));
                break;
            case "console":
                builder.AddConsoleExporter();
                break;
            case "none":
            default:
                break;
        }

        var cacheOptions = new FusionCacheConfigOptions();
        configuration.GetSection(FusionCacheConfigOptions.SectionName).Bind(cacheOptions);

        if (cacheOptions.Enabled)
        {
            builder.AddFusionCacheInstrumentation();
        }

        configFunctions?.MetricsConfiguration?.Invoke(builder);
    }

    private static void ConfigureLogging(LoggerProviderBuilder builder, ConfigurationManager configuration, TelemetryOptions options, ConfigurationFunctions? configFunctions)
    {
        switch (options.Log.Type)
        {
            case "otlp":
                builder.AddOtlpExporter(otlpOptions =>
                {
                    SetOltpOptions(configuration,  otlpOptions, options.Log.Otlp);
                });
                break;
            case "console":
                builder.AddConsoleExporter();
                break;
            case "none":
            default:
                break;
        }

        configFunctions?.LogConfiguration?.Invoke(builder);
    }


    private static void SetOltpOptions(ConfigurationManager configuration, OtlpExporterOptions otlpOptions, OtlpOptions options)
    {
        var endpoint = configuration.GetValue<string>("OTEL:Exporter:Otlp:Endpoint");
        var protocol = configuration.GetValue<string>("OTEL:Exporter:Otlp:Protocol") ?? "grpc";

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            endpoint = options.Endpoint;
        }

        if (string.IsNullOrWhiteSpace(protocol))
        {
            protocol = options.Protocol;
        }

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ConfigurationException("OTLP endpoint is required when using OTLP exporter.");
        }

        otlpOptions.Endpoint = new Uri(endpoint);
        otlpOptions.Headers = string.Join(",", options.Headers.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        if (protocol == "http")
        {
            otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
        }
        else
        {
            otlpOptions.Protocol = OtlpExportProtocol.Grpc;
        }
    }
    #endregion
}