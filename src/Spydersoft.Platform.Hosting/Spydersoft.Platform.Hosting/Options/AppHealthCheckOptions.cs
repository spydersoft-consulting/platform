namespace Spydersoft.Platform.Hosting.Options;

/// <summary>
/// Configuration options for application health checks.
/// Controls the behavior of Kubernetes-style health check endpoints (/readyz, /livez, /startup).
/// </summary>
public class AppHealthCheckOptions
{
    /// <summary>
    /// The configuration section name for binding.
    /// </summary>
    public const string SectionName = "HealthCheck";

    /// <summary>
    /// Gets or sets a value indicating whether health checks are enabled.
    /// When disabled, health check endpoints will not be configured.
    /// </summary>
    /// <value>
    /// <c>true</c> if health checks are enabled; otherwise, <c>false</c>. Default is <c>true</c>.
    /// </value>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the comma-separated tags for readiness checks.
    /// Health checks with matching tags will be included in the /readyz endpoint.
    /// </summary>
    /// <value>
    /// The ready tags. Default is "ready".
    /// </value>
    public string ReadyTags { get; set; } = "ready";

    /// <summary>
    /// Gets or sets the comma-separated tags for liveness checks.
    /// Health checks with matching tags will be included in the /livez endpoint.
    /// </summary>
    /// <value>
    /// The live tags. Default is "live".
    /// </value>
    public string LiveTags { get; set; } = "live";

    /// <summary>
    /// Gets or sets the comma-separated tags for startup checks.
    /// Health checks with matching tags will be included in the /startup endpoint.
    /// </summary>
    /// <value>
    /// The startup tags. Default is "startup".
    /// </value>
    public string StartupTags { get; set; } = "startup";

    /// <summary>
    /// Gets the ready tags as a list.
    /// Splits the <see cref="ReadyTags"/> by comma and removes empty entries.
    /// </summary>
    /// <returns>A list of ready tag strings.</returns>
    public List<string> ReadyTagsList()
    {
        return [.. ReadyTags.Split(',', StringSplitOptions.RemoveEmptyEntries)];
    }
    
    /// <summary>
    /// Gets the live tags as a list.
    /// Splits the <see cref="LiveTags"/> by comma and removes empty entries.
    /// </summary>
    /// <returns>A list of live tag strings.</returns>
    public List<string> LiveTagsList()
    {
        return [.. LiveTags.Split(',', StringSplitOptions.RemoveEmptyEntries)];
    }
    
    /// <summary>
    /// Gets the startup tags as a list.
    /// Splits the <see cref="StartupTags"/> by comma and removes empty entries.
    /// </summary>
    /// <returns>A list of startup tag strings.</returns>
    public List<string> StartupTagsList()

    {
        return [.. StartupTags.Split(',', StringSplitOptions.RemoveEmptyEntries)];
    }

}