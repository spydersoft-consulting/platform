using System.Net;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests;
public class DisabledConfigurationTests : ApiTestBase
{
    public override string Environment => "Disabled";

    [Test]
    [TestCase("livez")]
    [TestCase("readyz")]
    [TestCase("startup")]
    public async Task ShouldReturn_Http404(string healthEndpoint)
    {
        var result = await Client.GetAsync($"{healthEndpoint}");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}