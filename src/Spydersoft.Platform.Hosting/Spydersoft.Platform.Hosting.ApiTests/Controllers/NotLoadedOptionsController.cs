using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Spydersoft.Platform.Hosting.ApiTests.OptionsTests;

namespace Spydersoft.Platform.Hosting.ApiTests.Controllers;

[ApiController]
[Route("[controller]")]
public class NotLoadedOptionsController(IOptions<TaggedNotLoadedOptions> notLoadedOptions) : ControllerBase
{
    [HttpGet()]
    public TaggedNotLoadedOptions GetNotLoaded()
    {
        return notLoadedOptions.Value;
    }
}
