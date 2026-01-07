using System.Text.Json.Serialization;

namespace Spydersoft.Platform.Hosting.HealthChecks;

/// <summary>
/// Represents the result of an individual health check.
/// Contains status, description, duration, and additional data.
/// </summary>
public class HealthCheckResult
{
    /// <summary>
    /// Gets or sets the health check description.
    /// </summary>
    /// <value>
    /// A human-readable description of the health check.
    /// </value>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the health check status.
    /// </summary>
    /// <value>
    /// The status (e.g., "Healthy", "Degraded", "Unhealthy").
    /// </value>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the duration of the health check execution.
    /// </summary>
    /// <value>
    /// A string representation of the check duration.
    /// </value>
    public string Duration { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional data associated with the health check.
    /// </summary>
    /// <value>
    /// A dictionary of key-value pairs containing diagnostic information.
    /// </value>
    [JsonConverter(typeof(HealthCheckDataPropertyConvertor))]
    public IReadOnlyDictionary<string, object> ResultData { get; set; } = new Dictionary<string, object>();
}