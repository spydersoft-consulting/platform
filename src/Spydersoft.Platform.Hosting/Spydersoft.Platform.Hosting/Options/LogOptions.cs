namespace Spydersoft.Platform.Hosting.Options;
public class LogOptions
{
    public string Type { get; set; } = "console";
    public OtlpOptions Otlp { get; set; } = new OtlpOptions();

}