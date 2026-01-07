using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace Spydersoft.Platform.Attributes;

/// <summary>
/// Attribute for marking health check classes to be automatically discovered and registered.
/// Classes decorated with this attribute will be added to the health check pipeline at application startup.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class HealthCheckAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the health check.</param>
    /// <param name="failureStatus">The health status to report when the check fails.</param>
    /// <param name="tags">Comma-separated tags for categorizing the health check (e.g., "ready", "live", "startup").</param>
    public HealthCheckAttribute(string name, HealthStatus failureStatus, string tags = "")
    {
        Name = name;
        FailureStatus = failureStatus;
        RawTags = tags;
        Tags = tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }
    
    /// <summary>
    /// Gets the name of the health check.
    /// </summary>
    /// <value>
    /// The health check name.
    /// </value>
    public string Name { get; }

    /// <summary>
    /// Gets the health status to report when the check fails.
    /// </summary>
    /// <value>
    /// The failure status (e.g., Unhealthy, Degraded).
    /// </value>
    public HealthStatus? FailureStatus { get; }

    /// <summary>
    /// Gets the raw comma-separated tags string.
    /// </summary>
    /// <value>
    /// The raw tags string.
    /// </value>
    public string RawTags { get; }

    /// <summary>
    /// Gets the parsed array of tags.
    /// </summary>
    /// <value>
    /// An array of tag strings.
    /// </value>
    public string[] Tags { get; }
}
