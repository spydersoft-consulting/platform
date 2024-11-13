namespace Spydersoft.Platform.Hosting.Options;
public class MetricsOptions
{
    public string HistogramAggregation { get; set; } = string.Empty;
    public OtlpOptions Otlp { get; set; } = new OtlpOptions();
    public string Type { get; set; } = "console";

}