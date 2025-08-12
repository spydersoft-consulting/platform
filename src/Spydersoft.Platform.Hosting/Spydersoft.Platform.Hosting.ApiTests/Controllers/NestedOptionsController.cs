using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Spydersoft.Platform.Hosting.ApiTests.OptionsTests;

namespace Spydersoft.Platform.Hosting.ApiTests.Controllers;

[ApiController]
[Route("[controller]")]
public class NestedOptionsController(IOptions<NestedOptionSection> nestedOptions) : ControllerBase
{
    [HttpGet()]
    public NestedOptionSection GetNested()
    {
        return nestedOptions.Value;
    }
}
