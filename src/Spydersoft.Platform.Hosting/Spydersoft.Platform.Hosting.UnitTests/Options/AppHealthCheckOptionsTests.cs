using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.UnitTests.Options;

internal class AppHealthCheckOptionsTests
{
    [Test]
    public void AppHealthCheckOptionsDefaults()
    {
        var options = new AppHealthCheckOptions();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.Enabled, Is.True);
            Assert.That(options.ReadyTags, Is.EqualTo("ready"));
            Assert.That(options.LiveTags, Is.EqualTo("live"));
            Assert.That(options.StartupTags, Is.EqualTo("startup"));
        }
    }

    [Test]
    public void AppHealthCheckOptions_VerifyReadyTagsList()
    {
        string tagList = "ready,ready2,,ready3";
        List<string> expectedList = ["ready", "ready2", "ready3"];

        var options = new AppHealthCheckOptions()
        {
            ReadyTags = tagList
        };
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.Enabled, Is.True);
            Assert.That(options.ReadyTags, Is.EqualTo(tagList));
            Assert.That(options.ReadyTagsList(), Has.Exactly(expectedList.Count).Items);
            Assert.That(options.ReadyTagsList(), Is.EquivalentTo(expectedList));
        }
    }

    [Test]
    public void AppHealthCheckOptions_VerifyLiveTagsList()
    {
        string tagList = "live,live2,,live3";
        List<string> expectedList = ["live", "live2", "live3"];

        var options = new AppHealthCheckOptions()
        {
            LiveTags = tagList
        };
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.Enabled, Is.True);
            Assert.That(options.LiveTags, Is.EqualTo(tagList));
            Assert.That(options.LiveTagsList(), Has.Exactly(expectedList.Count).Items);
            Assert.That(options.LiveTagsList(), Is.EquivalentTo(expectedList));
        }
    }

    [Test]
    public void AppHealthCheckOptions_VerifyStartupTagsList()
    {
        string tagList = "startup,startup2,,startup3";
        List<string> expectedList = ["startup", "startup2", "startup3"];

        var options = new AppHealthCheckOptions()
        {
            StartupTags = tagList
        };
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.Enabled, Is.True);
            Assert.That(options.StartupTags, Is.EqualTo(tagList));
            Assert.That(options.StartupTagsList(), Has.Exactly(expectedList.Count).Items);
            Assert.That(options.StartupTagsList(), Is.EquivalentTo(expectedList));
        }
    }
}