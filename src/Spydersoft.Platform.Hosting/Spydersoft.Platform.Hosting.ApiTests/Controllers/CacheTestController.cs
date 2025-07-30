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
        public CacheObjectOne Get(int id)
        {
            if (cache == null)
            {
                Task.Delay(2000).Wait();
                return new CacheObjectOne
                {
                    Name = $"Name{id}",
                    Age = id,
                    CreatedAt = DateTime.UtcNow
                };
            }

            return cache.GetOrSet<CacheObjectOne>($"CacheObjectOne:{id}",
                _ => {
                    Task.Delay(1000).Wait(); // Simulate a delay for cache population
                    return new CacheObjectOne
                    {
                        Name = $"Name{id}",
                        Age = id,
                        CreatedAt = DateTime.UtcNow
                    };
                },
                options => options.SetDuration(TimeSpan.FromSeconds(10)));
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
