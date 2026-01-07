namespace Spydersoft.Platform.Hosting.Options;

/// <summary>
/// Configuration options for OpenTelemetry tracing.
/// </summary>
public class TraceOptions
{
    /// <summary>
    /// Gets or sets the trace exporter type.
    /// Valid values are "console" (default), "zipkin", or "otlp".
    /// </summary>
    public string Type { get; set; } = "console";

    /// <summary>
    /// Gets or sets the OTLP configuration options for trace export.
    /// </summary>
    public OtlpOptions Otlp { get; set; } = new OtlpOptions();

    /// <summary>
    /// Gets the configuration section path for Zipkin exporter options.
    /// </summary>
    public string ZipkinConfigurationSection { get; } = $"{TelemetryOptions.SectionName}:Trace:Zipkin";
}