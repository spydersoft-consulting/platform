using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Spydersoft.Platform.Hosting.Attributes;
using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.HealthChecks.Telemetry;

[SpydersoftHealthCheck(nameof(TelemetryHealthCheck), HealthStatus.Unhealthy, "startup")]
public class TelemetryHealthCheck(IServiceProvider services, IOptions<TelemetryOptions> telemetryOptions) : IHealthCheck
{
    private readonly TelemetryOptions _telemetryOptions = telemetryOptions.Value;
    private readonly IServiceProvider _services = services;

    public Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var tracePresent = _services.GetService(typeof(OpenTelemetry.Trace.TracerProvider)) is OpenTelemetry.Trace.TracerProvider;
        var metricsPresent = _services.GetService(typeof(OpenTelemetry.Metrics.MeterProvider)) is OpenTelemetry.Metrics.MeterProvider;
        var logPresent = _services.GetService(typeof(OpenTelemetry.Logs.LoggerProvider)) is OpenTelemetry.Logs.LoggerProvider;

        var details = new TelemetryHealthCheckDetails()
        {
            ActivitySourceName = _telemetryOptions.ActivitySourceName,
            Enabled = _telemetryOptions.Enabled,
            MeterName = _telemetryOptions.MeterName,
            ServiceName = _telemetryOptions.ServiceName,
            LogExporter = _telemetryOptions.UseLogExporter,
            MetricsExporter = _telemetryOptions.UseMetricsExporter,
            TraceExporter = _telemetryOptions.UseTracingExporter,
            HistogramAggregation = _telemetryOptions.HistogramAggregation,
            Otlp = _telemetryOptions.Otlp,
            TracePresent = tracePresent,
            MetricsPresent = metricsPresent,
            LogPresent = logPresent
        };

        HealthStatus status = tracePresent && metricsPresent && logPresent ? HealthStatus.Healthy : HealthStatus.Degraded;

        return Task.FromResult(new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(status,
            "Telemetry Configuration Options",
            null,
            new Dictionary<string, object> {
                { "details", details },
                { "errors", new { } } }
            ));
    }
}