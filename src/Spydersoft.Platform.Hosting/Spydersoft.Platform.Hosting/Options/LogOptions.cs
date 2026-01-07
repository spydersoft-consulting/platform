namespace Spydersoft.Platform.Hosting.Options;

/// <summary>
/// Configuration options for OpenTelemetry logging.
/// Specifies the log exporter type and OTLP-specific settings.
/// </summary>
public class LogOptions
{
    /// <summary>
    /// Gets or sets the log exporter type.
    /// </summary>
    /// <value>
    /// The exporter type. Default is "console".
    /// </value>
    public string Type { get; set; } = "console";
    
    /// <summary>
    /// Gets or sets the OTLP exporter configuration.
    /// Only used when <see cref="Type"/> is set to "otlp".
    /// </summary>
    /// <value>
    /// The OTLP options.
    /// </value>
    public OtlpOptions Otlp { get; set; } = new OtlpOptions();

}