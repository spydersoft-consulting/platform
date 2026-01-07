using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Spydersoft.Platform.Attributes;
using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.HealthChecks.Telemetry;

/// <summary>
/// Health check that validates OpenTelemetry configuration and provider registration.
/// Checks whether TracerProvider, MeterProvider, and LoggerProvider are properly registered.
/// </summary>
[HealthCheck(nameof(TelemetryHealthCheck), HealthStatus.Unhealthy, "startup")]
public class TelemetryHealthCheck(IServiceProvider services, IOptions<TelemetryOptions> telemetryOptions) : IHealthCheck
{
    private readonly TelemetryOptions _telemetryOptions = telemetryOptions.Value;
    private readonly IServiceProvider _services = services;

    /// <summary>
    /// Checks the health of the telemetry configuration.
    /// Returns Healthy if all three providers (trace, metrics, logs) are present, otherwise Degraded.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the health check result.</returns>
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
            Log = _telemetryOptions.Log,
            Metrics = _telemetryOptions.Metrics,
            Trace = _telemetryOptions.Trace,
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