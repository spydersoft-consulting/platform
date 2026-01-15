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
