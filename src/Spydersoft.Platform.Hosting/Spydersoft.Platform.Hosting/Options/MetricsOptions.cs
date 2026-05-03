namespace Spydersoft.Platform.Hosting.Options;

/// <summary>
/// Configuration options for OpenTelemetry metrics.
/// </summary>
public class MetricsOptions
{
    /// <summary>
    /// Gets or sets the histogram aggregation strategy.
    /// Valid values are "exponential" (default, maps to Datadog distribution) or empty string for explicit bucket histograms.
    /// Explicit bucket histograms are split into .count/.sum/.bucket metrics by Datadog's OTLP intake and will not appear as distributions.
    /// </summary>
    public string HistogramAggregation { get; set; } = "exponential";

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