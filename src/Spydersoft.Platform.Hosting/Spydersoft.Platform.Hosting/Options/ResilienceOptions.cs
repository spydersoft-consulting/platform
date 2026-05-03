using Microsoft.Extensions.Http.Resilience;

namespace Spydersoft.Platform.Hosting.Options;

/// <summary>
/// Configuration options for HTTP client resilience.
/// </summary>
public class ResilienceOptions
{
    /// <summary>
    /// The configuration section name used to bind these options.
    /// </summary>
    public const string SectionName = "Resilience";

    /// <summary>
    /// Gets or sets a value indicating whether HTTP resilience is enabled.
    /// When disabled, no resilience pipeline is applied to HTTP clients.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the standard HTTP resilience pipeline options applied to all HTTP clients.
    /// Controls retry, circuit breaker, and timeout behavior.
    /// </summary>
    public HttpStandardResilienceOptions Standard { get; set; } = new();
}
