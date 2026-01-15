namespace Spydersoft.Platform.Telemetry;

/// <summary>
/// Defines a contract for a telemetry client that abstracts telemetry tracking functionality
/// from specific implementations (e.g., Application Insights, OpenTelemetry Metrics).
/// </summary>
/// <remarks>
/// This interface provides a vendor-neutral API for tracking metrics, events, dependencies,
/// exceptions, and traces without exposing underlying telemetry SDK types.
/// </remarks>
public interface ITelemetryClient
{
    #region Metrics

    /// <summary>
    /// Tracks a numeric metric value.
    /// </summary>
    /// <param name="name">The name of the metric.</param>
    /// <param name="value">The metric value.</param>
    /// <param name="properties">Optional properties to associate with the metric.</param>
    void TrackMetric(string name, double value, IDictionary<string, string>? properties = null);

    /// <summary>
    /// Records a value for a counter metric (monotonically increasing).
    /// </summary>
    /// <param name="name">The name of the counter.</param>
    /// <param name="value">The value to add to the counter (default is 1).</param>
    /// <param name="tags">Optional tags to associate with the measurement.</param>
    void RecordCounter(string name, long value = 1, IDictionary<string, object?>? tags = null);

    /// <summary>
    /// Records a value for a histogram metric (value distribution).
    /// </summary>
    /// <param name="name">The name of the histogram.</param>
    /// <param name="value">The value to record.</param>
    /// <param name="tags">Optional tags to associate with the measurement.</param>
    void RecordHistogram(string name, double value, IDictionary<string, object?>? tags = null);

    /// <summary>
    /// Records an observable gauge value (point-in-time measurement).
    /// </summary>
    /// <param name="name">The name of the gauge.</param>
    /// <param name="value">The current value.</param>
    /// <param name="tags">Optional tags to associate with the measurement.</param>
    void RecordGauge(string name, double value, IDictionary<string, object?>? tags = null);

    #endregion

    #region Events

    /// <summary>
    /// Tracks a custom event.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="properties">Optional properties to associate with the event.</param>
    /// <param name="metrics">Optional numeric measurements associated with the event.</param>
    void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null);

    #endregion

    #region Dependencies

    /// <summary>
    /// Tracks a dependency call (e.g., HTTP, database, queue).
    /// </summary>
    /// <param name="dependencyTypeName">The type of dependency (e.g., "HTTP", "SQL", "Queue").</param>
    /// <param name="target">The target of the dependency (e.g., server name, endpoint).</param>
    /// <param name="dependencyName">The name of the dependency operation.</param>
    /// <param name="data">Optional additional data about the dependency call.</param>
    /// <param name="startTime">The start time of the dependency call.</param>
    /// <param name="duration">The duration of the dependency call.</param>
    /// <param name="success">Whether the dependency call was successful.</param>
    /// <param name="properties">Optional properties to associate with the dependency.</param>
    void TrackDependency(
        string dependencyTypeName,
        string target,
        string dependencyName,
        string? data,
        DateTimeOffset startTime,
        TimeSpan duration,
        bool success,
        IDictionary<string, string>? properties = null);

    #endregion

    #region Exceptions

    /// <summary>
    /// Tracks an exception.
    /// </summary>
    /// <param name="exception">The exception to track.</param>
    /// <param name="properties">Optional properties to associate with the exception.</param>
    /// <param name="metrics">Optional numeric measurements associated with the exception.</param>
    void TrackException(Exception exception, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null);

    #endregion

    #region Request Tracking

    /// <summary>
    /// Tracks an incoming request.
    /// </summary>
    /// <param name="name">The name of the request.</param>
    /// <param name="startTime">The start time of the request.</param>
    /// <param name="duration">The duration of the request.</param>
    /// <param name="responseCode">The response code.</param>
    /// <param name="success">Whether the request was successful.</param>
    /// <param name="properties">Optional properties to associate with the request.</param>
    /// <param name="metrics">Optional numeric measurements associated with the request.</param>
    void TrackRequest(
        string name,
        DateTimeOffset startTime,
        TimeSpan duration,
        string responseCode,
        bool success,
        IDictionary<string, string>? properties = null,
        IDictionary<string, double>? metrics = null);

    #endregion

    #region Availability

    /// <summary>
    /// Tracks availability test results.
    /// </summary>
    /// <param name="name">The name of the availability test.</param>
    /// <param name="startTime">The start time of the test.</param>
    /// <param name="duration">The duration of the test.</param>
    /// <param name="runLocation">The location where the test ran.</param>
    /// <param name="success">Whether the test was successful.</param>
    /// <param name="message">Optional message about the test result.</param>
    /// <param name="properties">Optional properties to associate with the availability test.</param>
    void TrackAvailability(
        string name,
        DateTimeOffset startTime,
        TimeSpan duration,
        string runLocation,
        bool success,
        string? message = null,
        IDictionary<string, string>? properties = null);

    #endregion

    #region Lifecycle

    /// <summary>
    /// Flushes any buffered telemetry data.
    /// </summary>
    void Flush();

    /// <summary>
    /// Flushes any buffered telemetry data asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the flush operation.</returns>
    Task FlushAsync(CancellationToken cancellationToken = default);

    #endregion
}