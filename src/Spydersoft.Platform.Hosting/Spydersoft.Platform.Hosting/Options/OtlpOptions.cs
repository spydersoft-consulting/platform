namespace Spydersoft.Platform.Hosting.Options;

/// <summary>
/// Configuration options for OpenTelemetry Protocol (OTLP) exporters.
/// </summary>
public class OtlpOptions
{
    /// <summary>
    /// Gets or sets the OTLP endpoint URL.
    /// </summary>
    public string? Endpoint { get; set; } = null;

    /// <summary>
    /// Gets or sets the protocol to use for OTLP communication.
    /// Valid values are "grpc" (default) or "http".
    /// </summary>
    public string Protocol { get; set; } = "grpc";

    /// <summary>
    /// Gets or sets the headers to include with OTLP requests.
    /// Useful for authentication tokens and custom metadata.
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}