using Spydersoft.Platform.Telemetry;

namespace Spydersoft.Platform.UnitTests.TelemetryTests;

public class NullTelemetryClientTests
{
    private ITelemetryClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _client = NullTelemetryClient.Instance;
    }

    #region Singleton Tests

    [Test]
    public void Instance_ShouldNotBeNull()
    {
        // Arrange & Act & Assert
        Assert.That(NullTelemetryClient.Instance, Is.Not.Null);
    }

    [Test]
    public void Instance_ShouldReturnSameInstance()
    {
        // Arrange
        var instance1 = NullTelemetryClient.Instance;
        var instance2 = NullTelemetryClient.Instance;

        // Act & Assert
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void Instance_ShouldImplementITelemetryClient()
    {
        // Arrange & Act & Assert
        Assert.That(NullTelemetryClient.Instance, Is.InstanceOf<ITelemetryClient>());
    }

    #endregion

    #region Metric Tests

    [Test]
    public void TrackMetric_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client.TrackMetric("test.metric", 42.5));
    }

    [Test]
    public void TrackMetric_WithProperties_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, string>
        {
            ["key1"] = "value1"
        };

        // Act & Assert
        Assert.DoesNotThrow(() => _client.TrackMetric("test.metric", 100.0, properties));
    }

    [Test]
    public void RecordCounter_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client.RecordCounter("test.counter"));
    }

    [Test]
    public void RecordCounter_WithTags_ShouldNotThrow()
    {
        // Arrange
        var tags = new Dictionary<string, object?>
        {
            ["tag1"] = "value1"
        };

        // Act & Assert
        Assert.DoesNotThrow(() => _client.RecordCounter("test.counter", 5, tags));
    }

    [Test]
    public void RecordHistogram_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client.RecordHistogram("test.histogram", 42.5));
    }

    [Test]
    public void RecordGauge_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client.RecordGauge("test.gauge", 75.5));
    }

    #endregion

    #region Event Tests

    [Test]
    public void TrackEvent_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client.TrackEvent("test.event"));
    }

    [Test]
    public void TrackEvent_WithProperties_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, string> { ["prop1"] = "value1" };
        var metrics = new Dictionary<string, double> { ["metric1"] = 42.0 };

        // Act & Assert
        Assert.DoesNotThrow(() => _client.TrackEvent("test.event", properties, metrics));
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
        Assert.DoesNotThrow(() => _client.TrackDependency(
            "HTTP",
            "api.example.com",
            "GET /users",
            null,
            startTime,
            duration,
            true));
    }

    #endregion

    #region Exception Tests

    [Test]
    public void TrackException_ShouldNotThrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");

        // Act & Assert
        Assert.DoesNotThrow(() => _client.TrackException(exception));
    }

    [Test]
    public void TrackException_WithProperties_ShouldNotThrow()
    {
        // Arrange
        var exception = new ArgumentException("Invalid argument");
        var properties = new Dictionary<string, string> { ["context"] = "test" };
        var metrics = new Dictionary<string, double> { ["error.count"] = 1 };

        // Act & Assert
        Assert.DoesNotThrow(() => _client.TrackException(exception, properties, metrics));
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
        Assert.DoesNotThrow(() => _client.TrackRequest(
            "GET /api/users",
            startTime,
            duration,
            "200",
            true));
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
        Assert.DoesNotThrow(() => _client.TrackAvailability(
            "HealthCheck",
            startTime,
            duration,
            "local",
            true));
    }

    #endregion

    #region Flush Tests

    [Test]
    public void Flush_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _client.Flush());
    }

    [Test]
    public async Task FlushAsync_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrowAsync(async () => await _client.FlushAsync());
    }

    [Test]
    public async Task FlushAsync_ShouldCompleteImmediately()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        await _client.FlushAsync();
        var endTime = DateTime.UtcNow;

        // Assert - should complete in less than 100ms
        var duration = endTime - startTime;
        Assert.That(duration.TotalMilliseconds, Is.LessThan(100));
    }

    #endregion

    #region Performance Tests

    [Test]
    public void MultipleCalls_ShouldHaveMinimalOverhead()
    {
        // Arrange
        var iterations = 10000;
        var startTime = DateTime.UtcNow;

        // Act
        for (int i = 0; i < iterations; i++)
        {
            _client.RecordCounter("test.counter", 1);
            _client.RecordHistogram("test.histogram", i);
            _client.TrackMetric("test.metric", i);
        }

        var endTime = DateTime.UtcNow;
        var duration = endTime - startTime;

        // Assert - should complete quickly (no-op implementation)
        Assert.That(duration.TotalMilliseconds, Is.LessThan(100),
            $"10,000 calls took {duration.TotalMilliseconds}ms, expected < 100ms");
    }

    #endregion
}
