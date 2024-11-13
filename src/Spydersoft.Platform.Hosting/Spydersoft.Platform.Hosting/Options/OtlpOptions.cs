namespace Spydersoft.Platform.Hosting.Options;

public class OtlpOptions
{
    public string? Endpoint { get; set; } = null;
    public string Protocol { get; set; } = "grpc";
}