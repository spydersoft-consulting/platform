using Spydersoft.Platform.Hosting.HealthChecks;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Identity;
public class IdentityConfigurationTests : ApiTestBase
{
    public override string Environment => "Identity";

    [Test]
    public async Task Livez_ShouldReturn_Http200()
    {
        var result = await Client.GetAsync($"livez");

        using var jsonResult = JsonDocument.Parse(await result.Content.ReadAsStringAsync());

        var telemetryNode = jsonResult.RootElement;

        var details = telemetryNode.Deserialize<HealthCheckResponseResult>(
                JsonOptions
        );
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(details, Is.Not.Null);
            Assert.That(details?.Status, Is.EqualTo("Healthy"));
            Assert.That(details?.Results, Has.One.With.Property("Key").EqualTo("self"));
        });
    }

    [Test]
    public async Task WeatherForeacast_ShouldReturn_Http404()
    {
        var result = await Client.GetAsync($"weatherforecast");
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}