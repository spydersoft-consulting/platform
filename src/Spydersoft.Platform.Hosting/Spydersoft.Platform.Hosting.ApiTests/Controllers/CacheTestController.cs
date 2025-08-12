using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spydersoft.Platform.Hosting.ApiTests.Models;
using ZiggyCreatures.Caching.Fusion;

namespace Spydersoft.Platform.Hosting.ApiTests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheTestController(IFusionCache? cache = null) : ControllerBase
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

            return await cache.GetOrSetAsync<CacheObjectOne>($"CacheObjectOne:{id}",
                async _ => {
                    await Task.Delay(2000); // Simulate a delay for cache population
                    return new CacheObjectOne
                    {
                        Name = $"Name{id}",
                        Age = id,
                        CreatedAt = DateTime.UtcNow
                    };
                },
                options => {
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
}
