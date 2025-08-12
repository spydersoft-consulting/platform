using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.FusionCache;

public class L2Tests : ApiTestBase
{
	public override string Environment => "FusionCacheL2";

	[Test]
	public async Task L2_CacheEnabled()
	{
		var result = await Client.GetAsync($"/cachetest/HasCache");

		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		var content = await result.Content.ReadAsStringAsync();

		var firstResult = JsonSerializer.Deserialize<dynamic>(content, JsonOptions);
		Assert.That(firstResult, Is.Not.Null);
		Assert.That(firstResult.GetProperty("hasCache").GetBoolean(), Is.True);
	}

	[Test]
	public async Task L2CacheEnabled()
	{
		var result = await Client.GetAsync($"/cachetest/HasDistributedCache");

		Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		var content = await result.Content.ReadAsStringAsync();

		var firstResult = JsonSerializer.Deserialize<dynamic>(content, JsonOptions);
		Assert.That(firstResult, Is.Not.Null);
		Assert.That(firstResult.GetProperty("hasL2Cache").GetBoolean(), Is.True);
	}

	[Test]
	public async Task InitialLoad_Occurs()
	{
		var stopwatch = Stopwatch.StartNew();
		var result = await Client.GetAsync($"/cachetest/99");
		stopwatch.Stop();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(stopwatch.ElapsedMilliseconds, Is.InRange(2000, 5000), "Initial load should take longer than 2 seconds due to cache population delay, but less than 5 seconds, which is the load without cache.");
			Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			var content = await result.Content.ReadAsStringAsync();

			var firstResult = JsonSerializer.Deserialize<dynamic>(content, JsonOptions);
			Assert.That(firstResult, Is.Not.Null);
		}
	}
	
	[Test]
	public async Task CacheLoad_Occurs()
	{
		// First call populates the cache.
		_ = await Client.GetAsync($"/cachetest/100");

		var stopwatch = Stopwatch.StartNew();
		var result = await Client.GetAsync($"/cachetest/100");
		stopwatch.Stop();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(2000), "Second Load should take less than 2 seconds.");
			Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			var content = await result.Content.ReadAsStringAsync();

			var firstResult = JsonSerializer.Deserialize<dynamic>(content, JsonOptions);
			Assert.That(firstResult, Is.Not.Null);
		}
	}

}
