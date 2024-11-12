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
using Spydersoft.Platform.Hosting.Exceptions;
using Spydersoft.Platform.Hosting.Options;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace Spydersoft.Platform.Hosting.StartupExtensions;

public static class TelemetryExtensions
{
    #region Public Startup Extensions
    public static void AddSpydersoftSerilog(this WebApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddSerilog(config =>
        {
            config
            .ReadFrom.Configuration(appBuilder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);
        });
    }

    public static WebApplicationBuilder AddSpydersoftTelemetry(this WebApplicationBuilder appBuilder,
        Assembly startupAssembly,
        Action<TracerProviderBuilder>? additionalTraceConfiguration,
        Action<MeterProviderBuilder>? additionalMetricsConfiguration,
        Action<LoggerProviderBuilder>? additionalLogConfiguration)
    {
        var telemetryOptions = new TelemetryOptions();
        appBuilder.Configuration.GetSection(TelemetryOptions.SectionName).Bind(telemetryOptions);

        // Add TelemetryOptions to the service collection for the Healthcheck
        appBuilder.Services.Configure<TelemetryOptions>(appBuilder.Configuration.GetSection(TelemetryOptions.SectionName));

        if (!telemetryOptions.Enabled)
        {
            return appBuilder;
        }

        // Use IConfiguration binding for AspNetCore instrumentation options.
        appBuilder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(appBuilder.Configuration.GetSection(telemetryOptions.AspNetCoreInstrumentationSection));

        appBuilder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(
                    serviceName: telemetryOptions.ServiceName,
                    serviceVersion: startupAssembly.GetName().Version?.ToString() ?? "unknown",
                    serviceInstanceId: Environment.MachineName))
            .WithTracing(builder => ConfigureTracing(builder, appBuilder.Configuration, telemetryOptions, additionalTraceConfiguration))
            .WithMetrics(builder => ConfigureMetrics(builder, telemetryOptions, additionalMetricsConfiguration))
            .WithLogging(builder => ConfigureLogging(builder, telemetryOptions, additionalLogConfiguration));

        return appBuilder;
    }

    public static WebApplicationBuilder AddSpydersoftTelemetry(this WebApplicationBuilder appBuilder, Assembly startupAssembly)
    {
        return AddSpydersoftTelemetry(appBuilder, startupAssembly, null, null, null);
    }

    #endregion


    #region Private Startup Helpers
    private static void ConfigureTracing(TracerProviderBuilder builder, ConfigurationManager configuration, TelemetryOptions options, Action<TracerProviderBuilder>? action)
    {

        // Ensure the TracerProvider subscribes to any custom ActivitySources.
        builder
            .AddSource(options.ActivitySourceName)
            .SetSampler(new AlwaysOnSampler())
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation();

        switch (options.UseTracingExporter)
        {
            case "zipkin":
                builder.AddZipkinExporter();

                builder.ConfigureServices(services =>
                {
                    // Use IConfiguration binding for Zipkin exporter options.
                    services.Configure<ZipkinExporterOptions>(configuration.GetSection(options.ZipkinConfigurationSection));
                });
                break;

            case "otlp":
                builder.AddOtlpExporter(otlpOptions => SetOltpOptions(otlpOptions, options));
                break;

            default:
                builder.AddConsoleExporter();
                break;
        }

        action?.Invoke(builder);
    }

    private static void ConfigureMetrics(MeterProviderBuilder builder, TelemetryOptions options, Action<MeterProviderBuilder>? action)
    {
        builder
            .AddMeter(options.MeterName)
            .SetExemplarFilter(ExemplarFilterType.TraceBased)
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation();

        switch (options.HistogramAggregation)
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

        switch (options.UseMetricsExporter)
        {
            case "prometheus":
                builder.AddPrometheusExporter();
                break;
            case "otlp":
                builder.AddOtlpExporter(otlpOptions => SetOltpOptions(otlpOptions, options));
                break;
            default:
                builder.AddConsoleExporter();
                break;
        }

        action?.Invoke(builder);
    }

    private static void ConfigureLogging(LoggerProviderBuilder builder, TelemetryOptions options, Action<LoggerProviderBuilder>? action)
    {
        switch (options.UseLogExporter)
        {
            case "otlp":
                builder.AddOtlpExporter(otlpOptions => SetOltpOptions(otlpOptions, options));
                break;
            default:
                builder.AddConsoleExporter();
                break;
        }

        action?.Invoke(builder);
    }


    private static void SetOltpOptions(OtlpExporterOptions otlpOptions, TelemetryOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Otlp.Endpoint))
        {
            throw new ConfigurationException("OTLP endpoint is required when using OTLP exporter.");
        }
        otlpOptions.Endpoint = new Uri(options.Otlp.Endpoint);
    }
    #endregion
}