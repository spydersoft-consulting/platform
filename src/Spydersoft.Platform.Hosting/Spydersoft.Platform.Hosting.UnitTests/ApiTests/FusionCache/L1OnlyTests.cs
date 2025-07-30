using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.FusionCache;
public class L1OnlyCacheTests : ApiTestBase
{
	public override string Environment => "FusionNoCache";

	[Test]
	public async Task L1Only_CacheEnabled()
	{
		var result = await Client.GetAsync($"/cachetest/HasCache");

		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		var content = await result.Content.ReadAsStringAsync();

		var firstResult = JsonSerializer.Deserialize<dynamic>(content, JsonOptions);
		Assert.That(firstResult, Is.Not.Null);
		Assert.That(firstResult.GetProperty("hasCache").GetBoolean(), Is.True);
	}

	[Test]
	public async Task L1Only_L2CacheDisabled()
	{
		var result = await Client.GetAsync($"/cachetest/HasDistributedCache");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await result.Content.ReadAsStringAsync();

		var firstResult = JsonSerializer.Deserialize<dynamic>(content, JsonOptions);
        Assert.That(firstResult, Is.Not.Null);
        Assert.That(firstResult.GetProperty("hasL2Cache").GetBoolean(), Is.False);
	}
}
