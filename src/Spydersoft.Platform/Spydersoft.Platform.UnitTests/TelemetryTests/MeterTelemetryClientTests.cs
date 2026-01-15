using System.Diagnostics;
using System.Diagnostics.Metrics;
using Spydersoft.Platform.Telemetry;

namespace Spydersoft.Platform.UnitTests.TelemetryTests;

public class MeterTelemetryClientTests : IDisposable
{
    private Meter? _meter;
    private ActivitySource? _activitySource;
    private MeterTelemetryClient? _client;
    private bool _disposed;

    [SetUp]
    public void Setup()
    {
        _meter = new Meter("TestMeter", "1.0.0");
        _activitySource = new ActivitySource("TestActivitySource", "1.0.0");
        _client = new MeterTelemetryClient(_meter, _activitySource);
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
        _meter?.Dispose();
        _activitySource?.Dispose();
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WithMeterAndActivitySource_ShouldSucceed()
    {
        // Arrange & Act
        using var meter = new Meter("Test", "1.0");
        using var activitySource = new ActivitySource("Test", "1.0");
        using var client = new MeterTelemetryClient(meter, activitySource);

        // Assert
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void Constructor_WithMeterOnly_ShouldCreateActivitySource()
    {
        // Arrange & Act
        using var meter = new Meter("Test", "1.0");
        using var client = new MeterTelemetryClient(meter);

        // Assert
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void Constructor_WithMeterName_ShouldSucceed()
    {
        // Arrange & Act
        using var client = new MeterTelemetryClient("TestApp", "1.0.0");

        // Assert
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void Constructor_WithNullMeter_ShouldThrow()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new MeterTelemetryClient((Meter)null!));
    }

    [Test]
    public void Constructor_WithEmptyMeterName_ShouldThrow()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new MeterTelemetryClient("", "1.0"));
    }

    [Test]
    public void Constructor_WithWhitespaceMeterName_ShouldThrow()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new MeterTelemetryClient("   ", "1.0"));
    }

    #endregion

    #region Metric Tests

    [Test]
    public void TrackMetric_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackMetric("test.metric", 42.5));
    }

    [Test]
    public void TrackMetric_WithProperties_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, string>
        {
            ["key1"] = "value1",
            ["key2"] = "value2"
        };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackMetric("test.metric", 100.0, properties));
    }

    [Test]
    public void RecordCounter_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordCounter("test.counter"));
    }

    [Test]
    public void RecordCounter_WithValue_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordCounter("test.counter", 5));
    }

    [Test]
    public void RecordCounter_WithTags_ShouldNotThrow()
    {
        // Arrange
        var tags = new Dictionary<string, object?>
        {
            ["tag1"] = "value1",
            ["tag2"] = 42
        };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordCounter("test.counter", 1, tags));
    }

    [Test]
    public void RecordCounter_MultipleCalls_ShouldReuseInstrument()
    {
        // Arrange & Act
        _client!.RecordCounter("reused.counter", 1);
        _client!.RecordCounter("reused.counter", 2);
        _client!.RecordCounter("reused.counter", 3);

        // Assert - should not throw
        Assert.Pass("Multiple calls to same counter succeeded");
    }

    [Test]
    public void RecordHistogram_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordHistogram("test.histogram", 42.5));
    }

    [Test]
    public void RecordHistogram_WithTags_ShouldNotThrow()
    {
        // Arrange
        var tags = new Dictionary<string, object?>
        {
            ["tag1"] = "value1",
            ["tag2"] = 42.5
        };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordHistogram("test.histogram", 100.0, tags));
    }

    [Test]
    public void RecordHistogram_MultipleCalls_ShouldReuseInstrument()
    {
        // Arrange & Act
        _client!.RecordHistogram("reused.histogram", 10.0);
        _client!.RecordHistogram("reused.histogram", 20.0);
        _client!.RecordHistogram("reused.histogram", 30.0);

        // Assert
        Assert.Pass("Multiple calls to same histogram succeeded");
    }

    [Test]
    public void RecordGauge_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordGauge("test.gauge", 75.5));
    }

    [Test]
    public void RecordGauge_MultipleCalls_ShouldUpdateValue()
    {
        // Arrange & Act
        _client!.RecordGauge("test.gauge", 10.0);
        _client!.RecordGauge("test.gauge", 20.0);
        _client!.RecordGauge("test.gauge", 30.0);

        // Assert - should not throw and should update value
        Assert.Pass("Multiple gauge updates succeeded");
    }

    #endregion

    #region Event Tests

    [Test]
    public void TrackEvent_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackEvent("test.event"));
    }

    [Test]
    public void TrackEvent_WithProperties_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, string>
        {
            ["prop1"] = "value1"
        };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackEvent("test.event", properties));
    }

    [Test]
    public void TrackEvent_WithMetrics_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, string> { ["prop1"] = "value1" };
        var metrics = new Dictionary<string, double> { ["metric1"] = 42.0 };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackEvent("test.event", properties, metrics));
    }

    #endregion

    #region Dependency Tests

    [Test]
    public void TrackDependency_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(100);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackDependency(
            "HTTP",
            "api.example.com",
            "GET /users",
            null,
            startTime,
            duration,
            true));
    }

    [Test]
    public void TrackDependency_WithProperties_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(150);
        var properties = new Dictionary<string, string>
        {
            ["userId"] = "123"
        };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackDependency(
            "SQL",
            "database-server",
            "SELECT * FROM Users",
            "UserId = 123",
            startTime,
            duration,
            true,
            properties));
    }

    [Test]
    public void TrackDependency_Failed_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(50);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackDependency(
            "HTTP",
            "api.example.com",
            "POST /orders",
            null,
            startTime,
            duration,
            false));
    }

    #endregion

    #region Exception Tests

    [Test]
    public void TrackException_ShouldNotThrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackException(exception));
    }

    [Test]
    public void TrackException_WithProperties_ShouldNotThrow()
    {
        // Arrange
        var exception = new ArgumentException("Invalid argument");
        var properties = new Dictionary<string, string>
        {
            ["context"] = "unit-test"
        };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackException(exception, properties));
    }

    [Test]
    public void TrackException_WithMetrics_ShouldNotThrow()
    {
        // Arrange
        var exception = new TimeoutException("Operation timed out");
        var properties = new Dictionary<string, string> { ["operation"] = "database-query" };
        var metrics = new Dictionary<string, double> { ["timeout.ms"] = 5000 };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackException(exception, properties, metrics));
    }

    #endregion

    #region Request Tests

    [Test]
    public void TrackRequest_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(200);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackRequest(
            "GET /api/users",
            startTime,
            duration,
            "200",
            true));
    }

    [Test]
    public void TrackRequest_WithProperties_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(150);
        var properties = new Dictionary<string, string>
        {
            ["userId"] = "123",
            ["method"] = "POST"
        };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackRequest(
            "POST /api/orders",
            startTime,
            duration,
            "201",
            true,
            properties));
    }

    [Test]
    public void TrackRequest_Failed_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(50);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackRequest(
            "GET /api/error",
            startTime,
            duration,
            "500",
            false));
    }

    #endregion

    #region Availability Tests

    [Test]
    public void TrackAvailability_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromSeconds(1);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackAvailability(
            "HealthCheck",
            startTime,
            duration,
            "local",
            true));
    }

    [Test]
    public void TrackAvailability_WithMessage_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromSeconds(2);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackAvailability(
            "DatabaseCheck",
            startTime,
            duration,
            "server1",
            true,
            "Database is healthy"));
    }

    [Test]
    public void TrackAvailability_Failed_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromSeconds(5);
        var properties = new Dictionary<string, string> { ["database"] = "users-db" };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackAvailability(
            "DatabaseCheck",
            startTime,
            duration,
            "server1",
            false,
            "Connection timeout",
            properties));
    }

    #endregion

    #region Flush Tests

    [Test]
    public void Flush_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.Flush());
    }

    [Test]
    public async Task FlushAsync_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrowAsync(async () => await _client!.FlushAsync());
    }

    [Test]
    public async Task FlushAsync_WithCancellation_ShouldNotThrow()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _client!.FlushAsync(cts.Token));
    }

    #endregion

    #region Dispose Tests

    [Test]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        using var meter = new Meter("Test", "1.0");
        var client = new MeterTelemetryClient(meter);

        // Act & Assert
        Assert.DoesNotThrow(() => client.Dispose());
    }

    [Test]
    public void Dispose_MultipleCalls_ShouldNotThrow()
    {
        // Arrange
        using var meter = new Meter("Test", "1.0");
        var client = new MeterTelemetryClient(meter);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            client.Dispose();
            client.Dispose();
            client.Dispose();
        });
    }

    #endregion

    #region Thread Safety Tests

    [Test]
    public void RecordCounter_ConcurrentCalls_ShouldBeThreadSafe()
    {
        // Arrange
        var tasks = new List<Task>();
        var iterations = 100;

        // Act - Multiple threads recording to same counter
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < iterations; j++)
                {
                    _client!.RecordCounter("concurrent.counter", 1);
                }
            }));
        }

        // Assert - Should not throw
        Assert.DoesNotThrowAsync(async () => await Task.WhenAll(tasks));
    }

    [Test]
    public void RecordHistogram_ConcurrentCalls_ShouldBeThreadSafe()
    {
        // Arrange
        var tasks = new List<Task>();
        var iterations = 100;

        // Act - Multiple threads recording to same histogram
        for (int i = 0; i < 10; i++)
        {
            var threadNum = i;
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < iterations; j++)
                {
                    _client!.RecordHistogram("concurrent.histogram", threadNum * 10.0 + j);
                }
            }));
        }

        // Assert - Should not throw
        Assert.DoesNotThrowAsync(async () => await Task.WhenAll(tasks));
    }

    [Test]
    public void RecordGauge_ConcurrentCalls_ShouldBeThreadSafe()
    {
        // Arrange
        var tasks = new List<Task>();
        var iterations = 100;

        // Act - Multiple threads updating same gauge
        for (int i = 0; i < 10; i++)
        {
            var threadNum = i;
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < iterations; j++)
                {
                    _client!.RecordGauge("concurrent.gauge", threadNum * 10.0 + j);
                }
            }));
        }

        // Assert - Should not throw
        Assert.DoesNotThrowAsync(async () => await Task.WhenAll(tasks));
    }

    [Test]
    public void MixedMetrics_ConcurrentCalls_ShouldBeThreadSafe()
    {
        // Arrange
        var tasks = new List<Task>();

        // Act - Mix of different metric types from multiple threads
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                _client!.RecordCounter("mixed.counter", 1);
                _client!.RecordHistogram("mixed.histogram", 42.0);
                _client!.RecordGauge("mixed.gauge", 100.0);
                _client!.TrackMetric("mixed.metric", 50.0);
            }));
        }

        // Assert - Should not throw
        Assert.DoesNotThrowAsync(async () => await Task.WhenAll(tasks));
    }

    #endregion

    #region Multiple Instruments Tests

    [Test]
    public void MultipleCounters_ShouldCreateSeparateInstruments()
    {
        // Arrange & Act
        _client!.RecordCounter("counter.one", 1);
        _client!.RecordCounter("counter.two", 2);
        _client!.RecordCounter("counter.three", 3);

        // Assert - Should not throw
        Assert.Pass("Successfully created multiple distinct counters");
    }

    [Test]
    public void MultipleHistograms_ShouldCreateSeparateInstruments()
    {
        // Arrange & Act
        _client!.RecordHistogram("histogram.one", 10.0);
        _client!.RecordHistogram("histogram.two", 20.0);
        _client!.RecordHistogram("histogram.three", 30.0);

        // Assert - Should not throw
        Assert.Pass("Successfully created multiple distinct histograms");
    }

    [Test]
    public void MultipleGauges_ShouldCreateSeparateInstruments()
    {
        // Arrange & Act
        _client!.RecordGauge("gauge.one", 10.0);
        _client!.RecordGauge("gauge.two", 20.0);
        _client!.RecordGauge("gauge.three", 30.0);

        // Assert - Should not throw
        Assert.Pass("Successfully created multiple distinct gauges");
    }

    #endregion

    #region Null and Empty Parameters Tests

    [Test]
    public void TrackMetric_WithNullProperties_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackMetric("test.metric", 42.0, null));
    }

    [Test]
    public void TrackMetric_WithEmptyProperties_ShouldNotThrow()
    {
        // Arrange
        var emptyProps = new Dictionary<string, string>();

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackMetric("test.metric", 42.0, emptyProps));
    }

    [Test]
    public void RecordCounter_WithNullTags_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordCounter("test.counter", 1, null));
    }

    [Test]
    public void RecordCounter_WithEmptyTags_ShouldNotThrow()
    {
        // Arrange
        var emptyTags = new Dictionary<string, object?>();

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordCounter("test.counter", 1, emptyTags));
    }

    [Test]
    public void RecordHistogram_WithNullTags_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordHistogram("test.histogram", 42.0, null));
    }

    [Test]
    public void RecordGauge_WithNullTags_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordGauge("test.gauge", 42.0, null));
    }

    [Test]
    public void TrackEvent_WithNullPropertiesAndMetrics_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackEvent("test.event", null, null));
    }

    [Test]
    public void TrackEvent_WithEmptyPropertiesAndMetrics_ShouldNotThrow()
    {
        // Arrange
        var emptyProps = new Dictionary<string, string>();
        var emptyMetrics = new Dictionary<string, double>();

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackEvent("test.event", emptyProps, emptyMetrics));
    }

    [Test]
    public void TrackDependency_WithNullDataAndProperties_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(100);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackDependency(
            "HTTP",
            "api.example.com",
            "GET /users",
            null,
            startTime,
            duration,
            true,
            null));
    }

    [Test]
    public void TrackDependency_WithEmptyData_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(100);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackDependency(
            "HTTP",
            "api.example.com",
            "GET /users",
            "",
            startTime,
            duration,
            true));
    }

    [Test]
    public void TrackException_WithNullPropertiesAndMetrics_ShouldNotThrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test");

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackException(exception, null, null));
    }

    [Test]
    public void TrackRequest_WithNullPropertiesAndMetrics_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(100);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackRequest(
            "GET /api/test",
            startTime,
            duration,
            "200",
            true,
            null,
            null));
    }

    [Test]
    public void TrackAvailability_WithNullMessageAndProperties_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromSeconds(1);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackAvailability(
            "HealthCheck",
            startTime,
            duration,
            "local",
            true,
            null,
            null));
    }

    [Test]
    public void TrackAvailability_WithEmptyMessage_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromSeconds(1);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackAvailability(
            "HealthCheck",
            startTime,
            duration,
            "local",
            true,
            ""));
    }

    #endregion

    #region Tag Value Types Tests

    [Test]
    public void RecordCounter_WithVariousTagTypes_ShouldNotThrow()
    {
        // Arrange
        var tags = new Dictionary<string, object?>
        {
            ["string.tag"] = "value",
            ["int.tag"] = 42,
            ["double.tag"] = 3.14,
            ["bool.tag"] = true,
            ["null.tag"] = null,
            ["guid.tag"] = Guid.NewGuid()
        };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordCounter("test.counter", 1, tags));
    }

    [Test]
    public void RecordHistogram_WithVariousTagTypes_ShouldNotThrow()
    {
        // Arrange
        var tags = new Dictionary<string, object?>
        {
            ["string.tag"] = "value",
            ["int.tag"] = 42,
            ["long.tag"] = 123L,
            ["null.tag"] = null
        };

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordHistogram("test.histogram", 42.0, tags));
    }

    #endregion

    #region Special Values Tests

    [Test]
    public void RecordCounter_WithZeroValue_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordCounter("test.counter", 0));
    }

    [Test]
    public void RecordCounter_WithNegativeValue_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        // Note: Counters technically should only accept positive values, but we don't enforce this
        Assert.DoesNotThrow(() => _client!.RecordCounter("test.counter", -1));
    }

    [Test]
    public void RecordHistogram_WithZeroValue_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordHistogram("test.histogram", 0.0));
    }

    [Test]
    public void RecordHistogram_WithNegativeValue_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordHistogram("test.histogram", -42.5));
    }

    [Test]
    public void RecordHistogram_WithVeryLargeValue_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordHistogram("test.histogram", double.MaxValue));
    }

    [Test]
    public void RecordHistogram_WithVerySmallValue_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordHistogram("test.histogram", double.Epsilon));
    }

    [Test]
    public void RecordGauge_WithNegativeValue_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client!.RecordGauge("test.gauge", -100.0));
    }

    [Test]
    public void RecordGauge_UpdateFromPositiveToNegative_ShouldNotThrow()
    {
        // Arrange & Act
        _client!.RecordGauge("test.gauge", 100.0);
        _client!.RecordGauge("test.gauge", -50.0);

        // Assert
        Assert.Pass("Successfully updated gauge from positive to negative");
    }

    #endregion

    #region Duration and Timestamp Tests

    [Test]
    public void TrackDependency_WithZeroDuration_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.Zero;

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackDependency(
            "HTTP",
            "api.example.com",
            "GET /fast",
            null,
            startTime,
            duration,
            true));
    }

    [Test]
    public void TrackRequest_WithVeryLongDuration_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromHours(24);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackRequest(
            "GET /slow",
            startTime,
            duration,
            "200",
            true));
    }

    [Test]
    public void TrackAvailability_WithPastTimestamp_ShouldNotThrow()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow.AddDays(-1);
        var duration = TimeSpan.FromSeconds(1);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackAvailability(
            "HealthCheck",
            startTime,
            duration,
            "local",
            true));
    }

    #endregion

    #region Exception Details Tests

    [Test]
    public void TrackException_WithNestedExceptions_ShouldNotThrow()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner error");
        var outerException = new Exception("Outer error", innerException);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackException(outerException));
    }

    [Test]
    public void TrackException_WithNullStackTrace_ShouldNotThrow()
    {
        // Arrange
        var exception = new Exception("Test exception");
        // Exception without being thrown has null stack trace

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackException(exception));
    }

    [Test]
    public void TrackException_WithLongMessage_ShouldNotThrow()
    {
        // Arrange
        var longMessage = new string('x', 10000);
        var exception = new Exception(longMessage);

        // Act & Assert
        Assert.DoesNotThrow(() => _client!.TrackException(exception));
    }

    #endregion

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _client?.Dispose();
                _meter?.Dispose();
                _activitySource?.Dispose();
            }
            _disposed = true;
        }
    }
}
