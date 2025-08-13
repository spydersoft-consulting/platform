using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Spydersoft.Platform.Hosting.ApiTests.OptionsTests;

namespace Spydersoft.Platform.Hosting.ApiTests.Controllers;

[ApiController]
[Route("[controller]")]
public class RootOptionsController(IOptions<RootOptionSection> rootOptions) : ControllerBase
{
    [HttpGet()]
    public RootOptionSection GetRoot()
    {
        return rootOptions.Value;
    }
}
