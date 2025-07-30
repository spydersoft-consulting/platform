using Spydersoft.Platform.Exceptions;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.FusionCache;
public class L2InvalidConfigTests
{
	[Test]
	public void StartupFails_NoRedisConnectionString()
	{
        var factory = new UnitTestWebApplicationFactory("FusionCacheL2NoRedis");
        Assert.Throws<ConfigurationException>(() => 
		{
            factory.CreateClient();
        });
	}
}
