using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.HealthChecks.Telemetry;

/// <summary>
/// Detailed information about telemetry configuration and provider status.
/// Used in health check responses to provide diagnostic information.
/// </summary>
public class TelemetryHealthCheckDetails
{
    /// <summary>
    /// Gets or sets the activity source name for tracing.
    /// </summary>
    /// <value>
    /// The activity source name.
    /// </value>
    public string ActivitySourceName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether telemetry is enabled.
    /// </summary>
    /// <value>
    /// <c>true</c> if telemetry is enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Enabled { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the log configuration options.
    /// </summary>
    /// <value>
    /// The log options.
    /// </value>
    public LogOptions Log { get; set; } = new LogOptions();
    
    /// <summary>
    /// Gets or sets a value indicating whether the LoggerProvider is registered.
    /// </summary>
    /// <value>
    /// <c>true</c> if LoggerProvider is present; otherwise, <c>false</c>.
    /// </value>
    public bool LogPresent { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the meter name for metrics.
    /// </summary>
    /// <value>
    /// The meter name.
    /// </value>
    public string MeterName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the metrics configuration options.
    /// </summary>
    /// <value>
    /// The metrics options.
    /// </value>
    public MetricsOptions Metrics { get; set; } = new MetricsOptions();
    
    /// <summary>
    /// Gets or sets a value indicating whether the MeterProvider is registered.
    /// </summary>
    /// <value>
    /// <c>true</c> if MeterProvider is present; otherwise, <c>false</c>.
    /// </value>
    public bool MetricsPresent { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the OpenTelemetry service name.
    /// </summary>
    /// <value>
    /// The service name.
    /// </value>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the trace configuration options.
    /// </summary>
    /// <value>
    /// The trace options.
    /// </value>
    public TraceOptions Trace { get; set; } = new TraceOptions();

    /// <summary>
    /// Gets or sets a value indicating whether the TracerProvider is registered.
    /// </summary>
    /// <value>
    /// <c>true</c> if TracerProvider is present; otherwise, <c>false</c>.
    /// </value>
    public bool TracePresent { get; set; } = false;
}