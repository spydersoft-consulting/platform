namespace Spydersoft.Platform.Hosting.Options;

public class TelemetryOptions
{
    /// <summary>
    /// The section name
    /// </summary>
    public const string SectionName = "Telemetry";

    public string ZipkinConfigurationSection { get; set; } = $"{SectionName}:Zipkin";

    public string AspNetCoreInstrumentationSection { get; set; } = $"{SectionName}:AspNetCoreInstrumentation";

    public string ServiceName { get; set; } = "spydersoft-otel-service";

    public string ActivitySourceName { get; set; } = "spydersoft-otel-activity";

    public string MeterName { get; set; } = "spydersoft-otel-meter";

    public string UseTracingExporter { get; set; } = "console";

    public string UseMetricsExporter { get; set; } = "console";

    public string UseLogExporter { get; set; } = "console";

    public string HistogramAggregation { get; set; } = string.Empty;


    public OtlpOptions Otlp { get; set; } = new OtlpOptions();
}