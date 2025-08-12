using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.UnitTests.Options;

public class LogOptionsTests
{
    [Test]
    public void OptionsDefaults()
    {
        var options = new LogOptions();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.Type, Is.EqualTo("console"));
            Assert.That(options.Otlp.Endpoint, Is.Null);
            Assert.That(options.Otlp.Protocol, Is.EqualTo("grpc"));
        }
    }

    [Test]
    public void IdentityOptionsPropertyTest()
    {
        var options = new LogOptions
        {
            Type = "otlp",
            Otlp = new OtlpOptions
            {
                Endpoint = "http://localhost:4317",
                Protocol = "http"
            }
        };
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.Type, Is.EqualTo("otlp"));
            Assert.That(options.Otlp.Endpoint, Is.EqualTo("http://localhost:4317"));
            Assert.That(options.Otlp.Protocol, Is.EqualTo("http"));
        }
    }
}