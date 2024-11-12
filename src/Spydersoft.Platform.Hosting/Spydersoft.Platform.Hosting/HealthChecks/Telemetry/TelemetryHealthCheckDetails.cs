using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.HealthChecks.Telemetry;

public class TelemetryHealthCheckDetails
{
    public string ActivitySourceName { get; set; } = string.Empty;
    public bool Enabled { get; set; } = false;
    public string HistogramAggregation { get; set; } = string.Empty;
    public string LogExporter { get; set; } = string.Empty;
    public bool LogPresent { get; set; } = false;
    public string MeterName { get; set; } = string.Empty;
    public string MetricsExporter { get; set; } = string.Empty;
    public bool MetricsPresent { get; set; } = false;
    public OtlpOptions Otlp { get; set; } = new OtlpOptions();
    public string ServiceName { get; set; } = string.Empty;
    public string TraceExporter { get; set; } = string.Empty;
    public bool TracePresent { get; set; } = false;
}