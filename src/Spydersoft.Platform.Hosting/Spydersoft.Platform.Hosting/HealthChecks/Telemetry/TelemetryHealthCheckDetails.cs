using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.HealthChecks.Telemetry;

public class TelemetryHealthCheckDetails
{
    public string ActivitySourceName { get; set; } = string.Empty;
    public bool Enabled { get; set; } = false;
    public LogOptions Log { get; set; } = new LogOptions();
    public bool LogPresent { get; set; } = false;
    public string MeterName { get; set; } = string.Empty;
    public MetricsOptions Metrics { get; set; } = new MetricsOptions();
    public bool MetricsPresent { get; set; } = false;
    public string ServiceName { get; set; } = string.Empty;

    public TraceOptions Trace { get; set; } = new TraceOptions();

    public bool TracePresent { get; set; } = false;
}