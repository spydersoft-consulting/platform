using Microsoft.Extensions.DependencyInjection;
using Spydersoft.Platform.Telemetry;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Telemetry;

public class TelemetryClientRegistrationTests : ApiTestBase
{
    public override string Environment => "Default";

    [Test]
    public void AddSpydersoftTelemetry_ShouldRegisterMeter()
    {
        // Arrange & Act
        var meter = Factory.Services.GetService<Meter>();

        // Assert
        Assert.That(meter, Is.Not.Null);
    }

    [Test]
    public void AddSpydersoftTelemetry_ShouldRegisterActivitySource()
    {
        // Arrange & Act
        var activitySource = Factory.Services.GetService<ActivitySource>();

        // Assert
        Assert.That(activitySource, Is.Not.Null);
    }

    [Test]
    public void AddSpydersoftTelemetry_ShouldRegisterITelemetryClient()
    {
        // Arrange & Act
        var telemetryClient = Factory.Services.GetService<ITelemetryClient>();

        // Assert
        Assert.That(telemetryClient, Is.Not.Null);
    }

    [Test]
    public void ITelemetryClient_ShouldBeMeterTelemetryClient()
    {
        // Arrange & Act
        var telemetryClient = Factory.Services.GetService<ITelemetryClient>();

        // Assert
        Assert.That(telemetryClient, Is.InstanceOf<MeterTelemetryClient>());
    }

    [Test]
    public void ITelemetryClient_ShouldBeSingleton()
    {
        // Arrange & Act
        var client1 = Factory.Services.GetService<ITelemetryClient>();
        var client2 = Factory.Services.GetService<ITelemetryClient>();

        // Assert
        Assert.That(client1, Is.SameAs(client2));
    }

    [Test]
    public void Meter_ShouldBeSingleton()
    {
        // Arrange & Act
        var meter1 = Factory.Services.GetService<Meter>();
        var meter2 = Factory.Services.GetService<Meter>();

        // Assert
        Assert.That(meter1, Is.SameAs(meter2));
    }

    [Test]
    public void ActivitySource_ShouldBeSingleton()
    {
        // Arrange & Act
        var source1 = Factory.Services.GetService<ActivitySource>();
        var source2 = Factory.Services.GetService<ActivitySource>();

        // Assert
        Assert.That(source1, Is.SameAs(source2));
    }

    [Test]
    public void ITelemetryClient_RecordCounter_ShouldWork()
    {
        // Arrange
        var telemetryClient = Factory.Services.GetRequiredService<ITelemetryClient>();

        // Act & Assert
        Assert.DoesNotThrow(() => telemetryClient.RecordCounter("test.counter", 1));
    }

    [Test]
    public void ITelemetryClient_RecordHistogram_ShouldWork()
    {
        // Arrange
        var telemetryClient = Factory.Services.GetRequiredService<ITelemetryClient>();

        // Act & Assert
        Assert.DoesNotThrow(() => telemetryClient.RecordHistogram("test.histogram", 42.5));
    }

    [Test]
    public void ITelemetryClient_TrackEvent_ShouldWork()
    {
        // Arrange
        var telemetryClient = Factory.Services.GetRequiredService<ITelemetryClient>();

        // Act & Assert
        Assert.DoesNotThrow(() => telemetryClient.TrackEvent("test.event"));
    }

    [Test]
    public void ITelemetryClient_TrackException_ShouldWork()
    {
        // Arrange
        var telemetryClient = Factory.Services.GetRequiredService<ITelemetryClient>();
        var exception = new InvalidOperationException("Test exception");

        // Act & Assert
        Assert.DoesNotThrow(() => telemetryClient.TrackException(exception));
    }
}
