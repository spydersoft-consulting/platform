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

    public static WebApplicationBuilder AddSpydersoftTelemetry(this WebApplicationBuilder appBuilder, Assembly startupAssembly)
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
            .WithTracing(builder => ConfigureTracing(builder, appBuilder.Configuration, telemetryOptions))
            .WithMetrics(builder => ConfigureMetrics(builder, telemetryOptions))
            .WithLogging(builder => ConfigureLogging(builder, telemetryOptions));

        return appBuilder;
    }

    #endregion


    #region Private Startup Helpers
    private static void ConfigureTracing(TracerProviderBuilder builder, ConfigurationManager configuration, TelemetryOptions options)
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
    }

    private static void ConfigureMetrics(MeterProviderBuilder builder, TelemetryOptions options)
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
    }

    private static void ConfigureLogging(LoggerProviderBuilder builder, TelemetryOptions options)
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
    }


    private static void SetOltpOptions(OtlpExporterOptions otlpOptions, TelemetryOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Otlp.Endpoint))
        {
            throw new InvalidOperationException("OTLP endpoint is required when using OTLP exporter.");
        }
        otlpOptions.Endpoint = new Uri(options.Otlp.Endpoint);
    }
    #endregion
}