using Microsoft.AspNetCore.Mvc;

namespace Spydersoft.Platform.Hosting.ApiTests.Controllers;

[ApiController]
[Route("[controller]")]
public class ResilienceController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        using var client = httpClientFactory.CreateClient();
        return Ok(new { clientCreated = client != null });
    }
}
