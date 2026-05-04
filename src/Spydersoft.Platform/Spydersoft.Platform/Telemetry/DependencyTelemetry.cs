namespace Spydersoft.Platform.Telemetry;

/// <summary>
/// Encapsulates the data for a dependency tracking call.
/// </summary>
public record DependencyTelemetry(
    string DependencyTypeName,
    string Target,
    string DependencyName,
    string? Data,
    DateTimeOffset StartTime,
    TimeSpan Duration,
    bool Success,
    IDictionary<string, string>? Properties = null);
