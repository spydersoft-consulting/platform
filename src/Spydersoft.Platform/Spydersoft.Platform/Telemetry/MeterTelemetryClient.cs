using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Spydersoft.Platform.Telemetry;

/// <summary>
/// Implementation of <see cref="ITelemetryClient"/> using System.Diagnostics.Metrics.
/// </summary>
/// <remarks>
/// This implementation uses OpenTelemetry-compatible System.Diagnostics.Metrics
/// to provide a modern, standards-based telemetry interface.
/// </remarks>
public class MeterTelemetryClient : ITelemetryClient, IDisposable
{
    private readonly Meter _meter;
    private readonly ActivitySource _activitySource;
    private readonly Dictionary<string, Counter<long>> _counters = new();
    private readonly Dictionary<string, Histogram<double>> _histograms = new();
    private readonly Dictionary<string, ObservableGauge<double>> _gauges = new();
    private readonly Dictionary<string, double> _gaugeValues = new();
    private readonly object _lock = new();
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeterTelemetryClient"/> class.
    /// </summary>
    /// <param name="meter">The meter instance for recording metrics.</param>
    /// <param name="activitySource">Optional activity source for tracing. If not provided, one will be created.</param>
    public MeterTelemetryClient(Meter meter, ActivitySource? activitySource = null)
    {
        _meter = meter ?? throw new ArgumentNullException(nameof(meter));
        _activitySource = activitySource ?? new ActivitySource(meter.Name);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeterTelemetryClient"/> class with a meter name.
    /// </summary>
    /// <param name="meterName">The name of the meter.</param>
    /// <param name="version">Optional version of the meter.</param>
    public MeterTelemetryClient(string meterName, string? version = null)
    {
        if (string.IsNullOrWhiteSpace(meterName))
        {
            throw new ArgumentException("Meter name cannot be null or whitespace.", nameof(meterName));
        }

        _meter = new Meter(meterName, version);
        _activitySource = new ActivitySource(meterName, version);
    }

    /// <inheritdoc/>
    public void TrackMetric(string name, double value, IDictionary<string, string>? properties = null)
    {
        // For generic metrics, use a histogram
        var histogram = GetOrCreateHistogram(name);
        histogram.Record(value, ConvertToTagList(properties));
    }

    /// <inheritdoc/>
    public void RecordCounter(string name, long value = 1, IDictionary<string, object?>? tags = null)
    {
        var counter = GetOrCreateCounter(name);
        counter.Add(value, ConvertToTagList(tags));
    }

    /// <inheritdoc/>
    public void RecordHistogram(string name, double value, IDictionary<string, object?>? tags = null)
    {
        var histogram = GetOrCreateHistogram(name);
        histogram.Record(value, ConvertToTagList(tags));
    }

    /// <inheritdoc/>
    public void RecordGauge(string name, double value, IDictionary<string, object?>? tags = null)
    {
        lock (_lock)
        {
            _gaugeValues[name] = value;

            if (!_gauges.ContainsKey(name))
            {
                _gauges[name] = _meter.CreateObservableGauge(name, () => _gaugeValues.TryGetValue(name, out var val) ? val : 0.0);
            }
        }
    }

    /// <inheritdoc/>
    public void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
    {
        // Events can be tracked as activities with specific event tags
        using var activity = _activitySource.StartActivity($"Event.{eventName}", ActivityKind.Internal);
        if (activity != null)
        {
            activity.SetTag("event.name", eventName);
            AddActivityTags(activity, properties);

            if (metrics != null)
            {
                foreach (var metric in metrics)
                {
                    activity.SetTag($"metric.{metric.Key}", metric.Value);
                }
            }
        }
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
        using var activity = _activitySource.StartActivity($"Dependency.{dependencyName}", ActivityKind.Client, default(ActivityContext), startTime: startTime.UtcDateTime);
        if (activity != null)
        {
            activity.SetTag("dependency.type", dependencyTypeName);
            activity.SetTag("dependency.target", target);
            activity.SetTag("dependency.name", dependencyName);
            activity.SetTag("dependency.success", success);

            if (!string.IsNullOrEmpty(data))
            {
                activity.SetTag("dependency.data", data);
            }

            AddActivityTags(activity, properties);
            activity.SetEndTime(startTime.Add(duration).UtcDateTime);

            if (!success)
            {
                activity.SetStatus(ActivityStatusCode.Error);
            }
        }
    }

    /// <inheritdoc/>
    public void TrackException(Exception exception, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
    {
        using var activity = _activitySource.StartActivity("Exception", ActivityKind.Internal);
        if (activity != null)
        {
            activity.SetTag("exception.type", exception.GetType().FullName);
            activity.SetTag("exception.message", exception.Message);
            activity.SetTag("exception.stacktrace", exception.StackTrace);
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);

            AddActivityTags(activity, properties);

            if (metrics != null)
            {
                foreach (var metric in metrics)
                {
                    activity.SetTag($"metric.{metric.Key}", metric.Value);
                }
            }
        }

        // Also record as a counter
        RecordCounter("exceptions", 1, new Dictionary<string, object?>
        {
            ["exception.type"] = exception.GetType().Name
        });
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
        using var activity = _activitySource.StartActivity($"Request.{name}", ActivityKind.Server, default(ActivityContext), startTime: startTime.UtcDateTime);
        if (activity != null)
        {
            activity.SetTag("request.name", name);
            activity.SetTag("request.response_code", responseCode);
            activity.SetTag("request.success", success);

            AddActivityTags(activity, properties);

            if (metrics != null)
            {
                foreach (var metric in metrics)
                {
                    activity.SetTag($"metric.{metric.Key}", metric.Value);
                }
            }

            activity.SetEndTime(startTime.Add(duration).UtcDateTime);

            if (!success)
            {
                activity.SetStatus(ActivityStatusCode.Error);
            }
        }
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
        using var activity = _activitySource.StartActivity($"Availability.{name}", ActivityKind.Internal, default(ActivityContext), startTime: startTime.UtcDateTime);
        if (activity != null)
        {
            activity.SetTag("availability.name", name);
            activity.SetTag("availability.location", runLocation);
            activity.SetTag("availability.success", success);

            if (!string.IsNullOrEmpty(message))
            {
                activity.SetTag("availability.message", message);
            }

            AddActivityTags(activity, properties);
            activity.SetEndTime(startTime.Add(duration).UtcDateTime);

            if (!success)
            {
                activity.SetStatus(ActivityStatusCode.Error);
            }
        }
    }

    /// <inheritdoc/>
    public void Flush()
    {
        // System.Diagnostics.Metrics doesn't require flushing as it's pull-based
        // This is a no-op for compatibility
    }

    /// <inheritdoc/>
    public Task FlushAsync(CancellationToken cancellationToken = default)
    {
        // System.Diagnostics.Metrics doesn't require flushing as it's pull-based
        return Task.CompletedTask;
    }

    #region Helper Methods

    private Counter<long> GetOrCreateCounter(string name)
    {
        lock (_lock)
        {
            if (!_counters.TryGetValue(name, out var counter))
            {
                counter = _meter.CreateCounter<long>(name);
                _counters[name] = counter;
            }
            return counter;
        }
    }

    private Histogram<double> GetOrCreateHistogram(string name)
    {
        lock (_lock)
        {
            if (!_histograms.TryGetValue(name, out var histogram))
            {
                histogram = _meter.CreateHistogram<double>(name);
                _histograms[name] = histogram;
            }
            return histogram;
        }
    }

    private static TagList ConvertToTagList(IDictionary<string, string>? properties)
    {
        var tagList = new TagList();
        if (properties != null)
        {
            foreach (var prop in properties)
            {
                tagList.Add(prop.Key, prop.Value);
            }
        }
        return tagList;
    }

    private static TagList ConvertToTagList(IDictionary<string, object?>? tags)
    {
        var tagList = new TagList();
        if (tags != null)
        {
            foreach (var tag in tags)
            {
                tagList.Add(tag.Key, tag.Value);
            }
        }
        return tagList;
    }

    private static void AddActivityTags(Activity activity, IDictionary<string, string>? properties)
    {
        if (properties == null)
        {
            return;
        }

        foreach (var property in properties)
        {
            activity.SetTag(property.Key, property.Value);
        }
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Disposes the telemetry client and associated resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the telemetry client and associated resources.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _meter?.Dispose();
            _activitySource?.Dispose();
        }

        _disposed = true;
    }

    #endregion
}
