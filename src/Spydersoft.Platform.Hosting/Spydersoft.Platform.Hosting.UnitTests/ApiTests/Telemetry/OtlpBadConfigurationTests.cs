using Spydersoft.Platform.Hosting.Exceptions;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Telemetry;
public class OtlpBadConfigurationTests
{

    [Test]
    public void ApplicationFails_WithException()
    {
        ConfigurationException ex = Assert.Throws<ConfigurationException>(() =>
        {
            var factory = new UnitTestWebApplicationFactory("OtlpBad");
            _ = factory.CreateClient();
        });

        Assert.That(ex.Message, Is.EqualTo("OTLP endpoint is required when using OTLP exporter."));
    }
}