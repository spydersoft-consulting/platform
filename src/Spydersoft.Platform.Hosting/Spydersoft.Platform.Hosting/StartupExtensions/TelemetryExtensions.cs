using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    /// <para>
    /// The console sink is for local/interactive tailing (e.g. <c>kubectl logs</c>) only — it uses a human-readable,
    /// colorized, multi-line template in every environment. It is <em>not</em> the collector ingestion path: log
    /// delivery to the backend (Loki, via Grafana Alloy) happens through the OpenTelemetry logging pipeline
    /// configured by <see cref="AddSpydersoftTelemetry(WebApplicationBuilder, Assembly)"/> (<c>Telemetry:Log:Type=otlp</c>),
    /// which receives events forwarded from Serilog when <paramref name="writeToProviders"/> is <c>true</c>. Do not
    /// reformat the console sink as structured JSON to try to make Grafana output more readable — that only helps if
    /// stdout is actually being scraped by a collector (check for a <c>logs.spydersoft.io/collect</c> pod annotation;
    /// most services don't have one and rely on OTLP export instead).
    /// </para>
    /// <para>
    /// When using this in conjunction with OpenTelemetry, make sure <paramref name="writeToProviders"/> is set to <c>true</c>,
    /// and call <see cref="AddSpydersoftTelemetry(WebApplicationBuilder, Assembly)"/> (or the overload with
    /// <see cref="ConfigurationFunctions"/>) <em>before</em> calling this method — <c>AddSpydersoftTelemetry</c> clears the
    /// default logging providers registered by <see cref="WebApplicationBuilder"/> so that Serilog's own console sink isn't
    /// duplicated by the framework's built-in console logger. Additionally, the Serilog configuration for log levels
    /// overrides any levels in the Logging section.
    /// </para>
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
    /// Adds Serilog's structured request-logging middleware, replacing ASP.NET Core's built-in
    /// per-request diagnostic logging (<c>Request starting</c>/<c>Request finished</c>) with a single
    /// structured log line per request (method, path, status code, elapsed time).
    /// </summary>
    /// <remarks>
    /// Call this once, immediately after <c>UseRouting()</c>. It replaces the need for hand-written
    /// "processing request" log lines in individual controller actions.
    /// </remarks>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseSpydersoftRequestLogging(this IApplicationBuilder app)
    {
        return app.UseSerilogRequestLogging();
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
        // WebApplicationBuilder.CreateBuilder() auto-registers a default console ILoggerProvider. If
        // AddSpydersoftSerilog(writeToProviders: true) is called afterward (as required whenever telemetry
        // is enabled, so Serilog events reach the OpenTelemetry logging bridge below), that default provider
        // would receive a second copy of every log event in a different format, duplicating console output.
        // Clearing providers here (before OpenTelemetry registers its own logging provider further down) removes
        // only that default provider. This requires AddSpydersoftTelemetry to be called before AddSpydersoftSerilog.
        appBuilder.Logging.ClearProviders();

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
            case "otlp":
                builder.AddOtlpExporter(otlpOptions => SetOltpOptions(configuration, otlpOptions, options.Trace.Otlp));
                break;

            case "console":
                builder.AddConsoleExporter();
                break;
            case "none":
                break;
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
                // Explicit bounds histogram - splits into .count/.sum/.bucket in Datadog's OTLP intake.
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
                    SetOltpOptions(configuration, otlpOptions, options.Log.Otlp);
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
        var endpoint = configuration.GetValue<string>("OTEL_EXPORTER_OTLP_ENDPOINT");
        var protocol = configuration.GetValue<string>("OTEL_EXPORTER_OTLP_PROTOCOL") ?? "grpc";
        var headers = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");

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

        if (!string.IsNullOrWhiteSpace(headers))
        {
            otlpOptions.Headers = headers;
        }
        else if (options.Headers.Count > 0)
        {
            otlpOptions.Headers = string.Join(",", options.Headers.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }

        if (protocol is "http/protobuf" or "http")
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
