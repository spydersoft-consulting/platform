using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Spydersoft.Platform.Hosting.ApiTests.OptionsTests;

namespace Spydersoft.Platform.Hosting.ApiTests.Controllers;

[ApiController]
[Route("[controller]")]
public class UntaggedOptionsController(IOptions<UntaggedOptions> untaggedOptions) : ControllerBase
{
    [HttpGet()]
    public UntaggedOptions GetUntagged()
    {
        return untaggedOptions.Value;
    }
}
