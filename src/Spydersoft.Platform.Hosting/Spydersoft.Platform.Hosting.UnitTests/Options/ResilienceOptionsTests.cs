using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.UnitTests.Options;

public class ResilienceOptionsTests
{
    [Test]
    public void OptionsDefaults()
    {
        var options = new ResilienceOptions();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.Enabled, Is.True);
            Assert.That(options.Standard, Is.Not.Null);
            Assert.That(options.Standard.Retry, Is.Not.Null);
            Assert.That(options.Standard.CircuitBreaker, Is.Not.Null);
            Assert.That(options.Standard.TotalRequestTimeout, Is.Not.Null);
            Assert.That(options.Standard.AttemptTimeout, Is.Not.Null);
        }
    }

    [Test]
    public void OptionsPropertyTest()
    {
        var options = new ResilienceOptions
        {
            Enabled = false
        };
        Assert.That(options.Enabled, Is.False);
    }

    [Test]
    public void SectionName_IsCorrect()
    {
        Assert.That(ResilienceOptions.SectionName, Is.EqualTo("Resilience"));
    }
}
