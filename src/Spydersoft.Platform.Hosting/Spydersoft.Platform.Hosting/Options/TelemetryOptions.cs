namespace Spydersoft.Platform.Hosting.Options;

/// <summary>
/// Configuration options for OpenTelemetry telemetry (tracing, metrics, and logging).
/// </summary>
public class TelemetryOptions
{
    /// <summary>
    /// The section name
    /// </summary>
    public const string SectionName = "Telemetry";

    /// <summary>
    /// Gets or sets the name of the OpenTelemetry ActivitySource for custom tracing.
    /// </summary>
    public string ActivitySourceName { get; set; } = "Spydersoft.Otel.Activity";

    /// <summary>
    /// Gets the configuration section path for ASP.NET Core instrumentation options.
    /// </summary>
    public string AspNetCoreInstrumentationSection { get; } = $"{SectionName}:AspNetCoreInstrumentation";

    /// <summary>
    /// Gets or sets a value indicating whether OpenTelemetry telemetry is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the name of the OpenTelemetry Meter for custom metrics.
    /// </summary>
    public string MeterName { get; set; } = "Spydersoft.Otel.Meter";

    /// <summary>
    /// Gets or sets the service name used in OpenTelemetry resource attributes.
    /// </summary>
    public string ServiceName { get; set; } = "spydersoft-otel-service";

    /// <summary>
    /// Gets or sets the tracing configuration options.
    /// </summary>
    public TraceOptions Trace { get; set; } = new TraceOptions();

    /// <summary>
    /// Gets or sets the metrics configuration options.
    /// </summary>
    public MetricsOptions Metrics { get; set; } = new MetricsOptions();

    /// <summary>
    /// Gets or sets the logging configuration options.
    /// </summary>
    public LogOptions Log { get; set; } = new LogOptions();

}