namespace Spydersoft.Platform.Hosting.Options;

/// <summary>
/// Configuration options for OpenTelemetry tracing.
/// </summary>
public class TraceOptions
{
    /// <summary>
    /// Gets or sets the trace exporter type.
    /// Valid values are "console" (default), "otlp", or "none".
    /// </summary>
    public string Type { get; set; } = "console";

    /// <summary>
    /// Gets or sets the OTLP configuration options for trace export.
    /// </summary>
    public OtlpOptions Otlp { get; set; } = new OtlpOptions();
}
