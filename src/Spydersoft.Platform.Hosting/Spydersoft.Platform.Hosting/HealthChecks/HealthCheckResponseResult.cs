namespace Spydersoft.Platform.Hosting.HealthChecks;

/// <summary>
/// Represents the aggregated response for all health checks.
/// Contains overall status, total duration, and individual check results.
/// </summary>
public class HealthCheckResponseResult
{
    /// <summary>
    /// Gets or sets the overall health status.
    /// </summary>
    /// <value>
    /// The aggregated status across all health checks (e.g., "Healthy", "Degraded", "Unhealthy").
    /// </value>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the total duration for all health checks.
    /// </summary>
    /// <value>
    /// A string representation of the total execution time.
    /// </value>
    public string TotalDuration { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the individual health check results.
    /// </summary>
    /// <value>
    /// A dictionary mapping health check names to their results.
    /// </value>
    public Dictionary<string, HealthCheckResult>? Results { get; set; }
}