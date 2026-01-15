namespace Spydersoft.Platform.Telemetry;

/// <summary>
/// A no-op implementation of <see cref="ITelemetryClient"/> that performs no actual telemetry tracking.
/// </summary>
/// <remarks>
/// This implementation is useful for testing scenarios or when telemetry is disabled.
/// All methods are no-ops and return immediately without doing any work.
/// </remarks>
public class NullTelemetryClient : ITelemetryClient
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="NullTelemetryClient"/>.
    /// </summary>
    public static NullTelemetryClient Instance { get; } = new();

    /// <inheritdoc/>
    public void TrackMetric(string name, double value, IDictionary<string, string>? properties = null)
    {
        // No-op
    }

    /// <inheritdoc/>
    public void RecordCounter(string name, long value = 1, IDictionary<string, object?>? tags = null)
    {
        // No-op
    }

    /// <inheritdoc/>
    public void RecordHistogram(string name, double value, IDictionary<string, object?>? tags = null)
    {
        // No-op
    }

    /// <inheritdoc/>
    public void RecordGauge(string name, double value, IDictionary<string, object?>? tags = null)
    {
        // No-op
    }

    /// <inheritdoc/>
    public void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
    {
        // No-op
    }

    /// <inheritdoc/>
    public void TrackDependency(
        string dependencyTypeName,
        string target,
        string dependencyName,
        string? data,
        DateTimeOffset startTime,
        TimeSpan duration,
        bool success,
        IDictionary<string, string>? properties = null)
    {
        // No-op
    }

    /// <inheritdoc/>
    public void TrackException(Exception exception, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
    {
        // No-op
    }


    /// <inheritdoc/>
    public void TrackRequest(
        string name,
        DateTimeOffset startTime,
        TimeSpan duration,
        string responseCode,
        bool success,
        IDictionary<string, string>? properties = null,
        IDictionary<string, double>? metrics = null)
    {
        // No-op
    }

    /// <inheritdoc/>
    public void TrackAvailability(
        string name,
        DateTimeOffset startTime,
        TimeSpan duration,
        string runLocation,
        bool success,
        string? message = null,
        IDictionary<string, string>? properties = null)
    {
        // No-op
    }

    /// <inheritdoc/>
    public void Flush()
    {
        // No-op
    }

    /// <inheritdoc/>
    public Task FlushAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
