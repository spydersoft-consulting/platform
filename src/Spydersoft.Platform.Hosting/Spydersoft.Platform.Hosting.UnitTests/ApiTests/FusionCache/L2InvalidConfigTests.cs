using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.FusionCache;
public class L2InvalidConfigTests
{
	[Test]
	public async Task StartupFails_NoRedisConnectionString()
	{
        var factory = new UnitTestWebApplicationFactory("FusionCacheL2NoRedis");
        Assert.Throws<ArgumentException>(() => 
		{
            factory.CreateClient();
        });
	}
}
