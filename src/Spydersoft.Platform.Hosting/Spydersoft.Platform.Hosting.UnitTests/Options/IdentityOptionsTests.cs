using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.UnitTests.Options;

public class IdentityOptionsTests
{
    [Test]
    public void IdentityOptionsDefaults()
    {
        var options = new IdentityOptions();
        Assert.Multiple(() =>
        {
            Assert.That(options.Authority, Is.Null);
            Assert.That(options.ApplicationName, Is.EqualTo("spydersoft-application"));
        });
    }

    [Test]
    public void IdentityOptionsScheme()
    {
        Assert.That(IdentityOptions.SectionName, Is.EqualTo("Identity"));
    }

    [Test]
    public void IdentityOptionsPropertyTest()
    {
        var options = new IdentityOptions
        {
            Authority = "https://localhost:1234",
            ApplicationName = "test-client-id"
        };
        Assert.Multiple(() =>
        {
            Assert.That(options, Is.Not.Null);
            Assert.That(options, Has.Property("Authority").TypeOf<string>());
            Assert.That(options, Has.Property("ApplicationName").TypeOf<string>());
        });
    }
}