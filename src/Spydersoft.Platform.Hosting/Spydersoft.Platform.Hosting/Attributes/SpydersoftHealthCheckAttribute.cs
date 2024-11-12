using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Spydersoft.Platform.Hosting.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SpydersoftHealthCheckAttribute(string name, HealthStatus failureStatus, string tags = "") : Attribute
{
    public string Name { get; } = name;

    public HealthStatus? FailureStatus { get; } = failureStatus;

    public string RawTags { get; } = tags;

    public string[] Tags { get; } = tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
}