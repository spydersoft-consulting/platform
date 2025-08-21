using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Spydersoft.Platform.Hosting.ApiTests.Models;
using Spydersoft.Platform.Hosting.Options;
using ZiggyCreatures.Caching.Fusion;

namespace Spydersoft.Platform.Hosting.ApiTests.Controllers;

[ApiController]
[Route("[controller]")]
public class CacheTestController(IOptions<FusionCacheConfigOptions> options, IFusionCache? cache = null) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<CacheObjectOne> Get(int id)
    {
        if (cache == null)
        {
            await Task.Delay(5000);
            return new CacheObjectOne
            {
                Name = $"Name{id}",
                Age = id,
                CreatedAt = DateTime.UtcNow
            };
        }

        return await cache.GetOrSetAsync($"CacheObjectOne:{id}",
            async (token) =>
            {
                await Task.Delay(2000, token); // Simulate a delay for cache population
                return new CacheObjectOne
                {
                    Name = $"Name{id}",
                    Age = id,
                    CreatedAt = DateTime.UtcNow
                };
            },
            options =>
            {
                options.SetDuration(TimeSpan.FromSeconds(10))
                       .SetFactoryTimeouts(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));
            });
    }

    [HttpGet("HasDistributedCache")]
    public IActionResult HasDistributedCache()
    {
        return Ok(new { HasL2Cache = cache?.HasDistributedCache ?? false });
    }

    [HttpGet("HasCache")]
    public IActionResult HasCache()
    {
        return Ok(new { HasCache = cache != null });
    }
}
