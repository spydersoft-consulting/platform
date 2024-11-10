namespace Spydersoft.Platform.Hosting.Options;

public class TelemetryOptions
{
    /// <summary>
    /// The section name
    /// </summary>
    public const string SectionName = "Telemetry";

    public string ActivitySourceName { get; set; } = "Spydersoft.Otel.Activity";
    public string AspNetCoreInstrumentationSection { get; set; } = $"{SectionName}:AspNetCoreInstrumentation";
    public bool Enabled { get; set; } = true;
    public string HistogramAggregation { get; set; } = string.Empty;
    public string MeterName { get; set; } = "Spydersoft.Otel.Meter";
    public OtlpOptions Otlp { get; set; } = new OtlpOptions();
    public string ServiceName { get; set; } = "spydersoft-otel-service";
    public string UseLogExporter { get; set; } = "console";
    public string UseMetricsExporter { get; set; } = "console";
    public string UseTracingExporter { get; set; } = "console";
    public string ZipkinConfigurationSection { get; set; } = $"{SectionName}:Zipkin";
}