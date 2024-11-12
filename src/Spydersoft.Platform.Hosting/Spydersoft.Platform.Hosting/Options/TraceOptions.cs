namespace Spydersoft.Platform.Hosting.Options;

public class TraceOptions
{
    public string Type { get; set; } = "console";

    public OtlpOptions Otlp { get; set; } = new OtlpOptions();

    public string ZipkinConfigurationSection { get; set; } = $"{TelemetryOptions.SectionName}:Trace:Zipkin";
}