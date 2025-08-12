using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.FusionCache;

public class NoFusionCacheTests : ApiTestBase
{
	public override string Environment => "NoFusionCache";

	[Test]
	public async Task L1Only_CacheEnabled()
	{
		var result = await Client.GetAsync($"/cachetest/HasCache");

		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		var content = await result.Content.ReadAsStringAsync();

		var firstResult = JsonSerializer.Deserialize<dynamic>(content, JsonOptions);
		Assert.That(firstResult, Is.Not.Null);
		Assert.That(firstResult.GetProperty("hasCache").GetBoolean(), Is.False);
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
	
	[Test]
	public async Task InitialLoad_Occurs()
	{
		var stopwatch = Stopwatch.StartNew();
		var result = await Client.GetAsync($"/cachetest/9999");
		stopwatch.Stop();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThan(4990), "Initial load should take longer than 5 seconds due to no cache in place.");
			Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			var content = await result.Content.ReadAsStringAsync();

			var firstResult = JsonSerializer.Deserialize<dynamic>(content, JsonOptions);
			Assert.That(firstResult, Is.Not.Null);
		}
	}
}
