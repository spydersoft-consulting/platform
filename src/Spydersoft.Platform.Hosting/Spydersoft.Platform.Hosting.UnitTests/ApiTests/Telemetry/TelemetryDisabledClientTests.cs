using Microsoft.Extensions.DependencyInjection;
using Spydersoft.Platform.Telemetry;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Telemetry;

public class TelemetryDisabledClientTests : ApiTestBase
{
    public override string Environment => "TelemetryDisabled";

    [Test]
    public void TelemetryDisabled_ShouldRegisterNullTelemetryClient()
    {
        // Arrange & Act
        var telemetryClient = Factory.Services.GetService<ITelemetryClient>();

        // Assert
        Assert.That(telemetryClient, Is.Not.Null);
        Assert.That(telemetryClient, Is.InstanceOf<NullTelemetryClient>());
    }

    [Test]
    public void TelemetryDisabled_ShouldUseSingletonInstance()
    {
        // Arrange & Act
        var telemetryClient = Factory.Services.GetService<ITelemetryClient>();

        // Assert
        Assert.That(telemetryClient, Is.SameAs(NullTelemetryClient.Instance));
    }

    [Test]
    public void NullTelemetryClient_RecordCounter_ShouldNotThrow()
    {
        // Arrange
        var telemetryClient = Factory.Services.GetRequiredService<ITelemetryClient>();

        // Act & Assert
        Assert.DoesNotThrow(() => telemetryClient.RecordCounter("test.counter", 1));
    }

    [Test]
    public void NullTelemetryClient_RecordHistogram_ShouldNotThrow()
    {
        // Arrange
        var telemetryClient = Factory.Services.GetRequiredService<ITelemetryClient>();

        // Act & Assert
        Assert.DoesNotThrow(() => telemetryClient.RecordHistogram("test.histogram", 42.5));
    }

    [Test]
    public void NullTelemetryClient_TrackMetric_ShouldNotThrow()
    {
        // Arrange
        var telemetryClient = Factory.Services.GetRequiredService<ITelemetryClient>();

        // Act & Assert
        Assert.DoesNotThrow(() => telemetryClient.TrackMetric("test.metric", 100.0));
    }

    [Test]
    public void NullTelemetryClient_TrackEvent_ShouldNotThrow()
    {
        // Arrange
        var telemetryClient = Factory.Services.GetRequiredService<ITelemetryClient>();

        // Act & Assert
        Assert.DoesNotThrow(() => telemetryClient.TrackEvent("test.event"));
    }

    [Test]
    public void NullTelemetryClient_TrackException_ShouldNotThrow()
    {
        // Arrange
        var telemetryClient = Factory.Services.GetRequiredService<ITelemetryClient>();
        var exception = new InvalidOperationException("Test exception");

        // Act & Assert
        Assert.DoesNotThrow(() => telemetryClient.TrackException(exception));
    }

    [Test]
    public void NullTelemetryClient_TrackDependency_ShouldNotThrow()
    {
        // Arrange
        var telemetryClient = Factory.Services.GetRequiredService<ITelemetryClient>();
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(100);

        // Act & Assert
        Assert.DoesNotThrow(() => telemetryClient.TrackDependency(
            "HTTP",
            "api.example.com",
            "GET /users",
            null,
            startTime,
            duration,
            true));
    }

    [Test]
    public async Task NullTelemetryClient_FlushAsync_ShouldCompleteQuickly()
    {
        // Arrange
        var telemetryClient = Factory.Services.GetRequiredService<ITelemetryClient>();
        var startTime = DateTime.UtcNow;

        // Act
        await telemetryClient.FlushAsync();
        var endTime = DateTime.UtcNow;

        // Assert
        var duration = endTime - startTime;
        Assert.That(duration.TotalMilliseconds, Is.LessThan(100));
    }
}
