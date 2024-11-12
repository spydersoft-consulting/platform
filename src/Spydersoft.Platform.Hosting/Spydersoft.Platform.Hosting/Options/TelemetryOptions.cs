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
    public string MeterName { get; set; } = "Spydersoft.Otel.Meter";
    public string ServiceName { get; set; } = "spydersoft-otel-service";

    public TraceOptions Trace { get; set; } = new TraceOptions();

    public MetricsOptions Metrics { get; set; } = new MetricsOptions();

    public LogOptions Log { get; set; } = new LogOptions();

}