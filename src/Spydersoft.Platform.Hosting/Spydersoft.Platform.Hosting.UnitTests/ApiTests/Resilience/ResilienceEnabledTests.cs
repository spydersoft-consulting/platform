using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Resilience;

public class ResilienceEnabledTests : ApiTestBase
{
    public override string Environment => "Production";

    [Test]
    public async Task Resilience_Enabled_ClientCreated()
    {
        var result = await Client.GetAsync("resilience");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var jsonResult = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
        Assert.That(jsonResult.RootElement.GetProperty("clientCreated").GetBoolean(), Is.True);
    }
}
