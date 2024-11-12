using Spydersoft.Platform.Hosting.HealthChecks;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests;
public class EmptyHealthCheckTests : ApiTestBase
{
    public override string Environment => "Empty";

    [Test]
    [TestCase("livez")]
    [TestCase("readyz")]
    [TestCase("startup")]
    public async Task ShouldReturn_Http200(string healthEndpoint)
    {
        var result = await Client.GetAsync($"{healthEndpoint}");

        using var jsonResult = JsonDocument.Parse(await result.Content.ReadAsStringAsync());

        var telemetryNode = jsonResult.RootElement;

        var responseResult = telemetryNode.Deserialize<HealthCheckResponseResult>(
                JsonOptions
        );
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseResult, Is.Not.Null);
            Assert.That(responseResult?.Status, Is.EqualTo("Healthy"));
            Assert.That(responseResult?.Results, Is.Empty);
        });
    }


}