using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.UnitTests.Options;

public class MetricsOptionsTests
{
    [Test]
    public void OptionsDefaults()
    {
        var options = new MetricsOptions();
        Assert.Multiple(() =>
        {
            Assert.That(options.Type, Is.EqualTo("console"));
            Assert.That(options.HistogramAggregation, Is.EqualTo(string.Empty));
            Assert.That(options.Otlp.Endpoint, Is.Null);
            Assert.That(options.Otlp.Protocol, Is.EqualTo("grpc"));
        });
    }

    [Test]
    public void IdentityOptionsPropertyTest()
    {
        var options = new MetricsOptions
        {
            Type = "otlp",
            HistogramAggregation = "exponential",
            Otlp = new OtlpOptions
            {
                Endpoint = "http://localhost:4317",
                Protocol = "http"
            }
        };
        Assert.Multiple(() =>
        {
            Assert.That(options.Type, Is.EqualTo("otlp"));
            Assert.That(options.HistogramAggregation, Is.EqualTo("exponential"));
            Assert.That(options.Otlp.Endpoint, Is.EqualTo("http://localhost:4317"));
            Assert.That(options.Otlp.Protocol, Is.EqualTo("http"));
        });
    }
}