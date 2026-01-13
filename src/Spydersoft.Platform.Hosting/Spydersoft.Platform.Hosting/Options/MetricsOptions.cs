namespace Spydersoft.Platform.Hosting.Options;

/// <summary>
/// Configuration options for OpenTelemetry metrics.
/// </summary>
public class MetricsOptions
{
    /// <summary>
    /// Gets or sets the histogram aggregation strategy.
    /// Valid values are empty string (default explicit bounds) or "exponential" for exponential bucket histograms.
    /// </summary>
    public string HistogramAggregation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OTLP configuration options for metrics export.
    /// </summary>
    public OtlpOptions Otlp { get; set; } = new OtlpOptions();

    /// <summary>
    /// Gets or sets the metrics exporter type.
    /// Valid values are "console" (default), "prometheus", "otlp", or "none".
    /// </summary>
    public string Type { get; set; } = "console";

}