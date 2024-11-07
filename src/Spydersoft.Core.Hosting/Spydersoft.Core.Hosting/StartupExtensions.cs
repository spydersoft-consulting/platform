using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Spydersoft.Core.Hosting.Options;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace Spydersoft.Core.Hosting;

public static class StartupExtensions
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

    public static void AddSpydersoftTelemetry(this WebApplicationBuilder appBuilder, Assembly startupAssembly)
    {
        var telemetryOptions = new TelemetryOptions();
        appBuilder.Configuration.GetSection(TelemetryOptions.SectionName).Bind(telemetryOptions);

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
    }

    public static bool AddSpydersoftIdentity(this WebApplicationBuilder appBuilder)
    {
        var authInstalled = false;
        var identityOption = new Options.IdentityOptions();
        appBuilder.Configuration.GetSection(Options.IdentityOptions.SectionName).Bind(identityOption);

        if (identityOption.Authority != null)
        {
            appBuilder.Services
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.IncludeErrorDetails = true;

                    o.Authority = identityOption.Authority;
                    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidAudiences = new List<string>
                                        {
                                            identityOption.ApplicationName
                                        },
                        ValidIssuers = new List<string>
                                        {
                                            identityOption.Authority
                                        }
                    };
                });

            appBuilder.Services.AddAuthorization();
            authInstalled = true;
        }
        return authInstalled;
    }

    public static IApplicationBuilder UseAuthentication(this IApplicationBuilder app, bool authInstalled)
    {
        if (authInstalled)
        {
            app.UseAuthentication();
        }
        return app;
    }

    public static IApplicationBuilder UseAuthorization(this IApplicationBuilder app, bool authInstalled)
    {
        if (authInstalled)
        {
            app.UseAuthorization();
        }
        return app;
    }


    #endregion


    #region Private Startup Helpers
    private static void ConfigureTracing(TracerProviderBuilder builder, IConfiguration configuration, TelemetryOptions options)
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