using Spydersoft.Platform.Telemetry;

namespace Spydersoft.Platform.UnitTests.TelemetryTests;

public class DependencyTelemetryTests
{
    [Test]
    public void DependencyTelemetry_ShouldSetAllProperties()
    {
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(150);
        var properties = new Dictionary<string, string> { ["region"] = "us-east" };

        var telemetry = new DependencyTelemetry(
            "HTTP", "api.example.com", "GET /users", "query=all",
            startTime, duration, true, properties);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(telemetry.DependencyTypeName, Is.EqualTo("HTTP"));
            Assert.That(telemetry.Target, Is.EqualTo("api.example.com"));
            Assert.That(telemetry.DependencyName, Is.EqualTo("GET /users"));
            Assert.That(telemetry.Data, Is.EqualTo("query=all"));
            Assert.That(telemetry.StartTime, Is.EqualTo(startTime));
            Assert.That(telemetry.Duration, Is.EqualTo(duration));
            Assert.That(telemetry.Success, Is.True);
            Assert.That(telemetry.Properties, Is.SameAs(properties));
        }
    }

    [Test]
    public void DependencyTelemetry_DefaultProperties_IsNull()
    {
        var telemetry = new DependencyTelemetry(
            "SQL", "db-server", "SELECT * FROM Orders", null,
            DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(50), false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(telemetry.Properties, Is.Null);
            Assert.That(telemetry.Data, Is.Null);
            Assert.That(telemetry.Success, Is.False);
        }
    }

    [Test]
    public void DependencyTelemetry_RecordEquality_SameValuesShouldBeEqual()
    {
        var startTime = DateTimeOffset.UtcNow;
        var duration = TimeSpan.FromMilliseconds(100);

        var a = new DependencyTelemetry("HTTP", "host", "GET /", null, startTime, duration, true);
        var b = new DependencyTelemetry("HTTP", "host", "GET /", null, startTime, duration, true);

        Assert.That(a, Is.EqualTo(b));
    }
}
