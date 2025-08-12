using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Spydersoft.Platform.Hosting.ApiTests.OptionsTests;

namespace Spydersoft.Platform.Hosting.ApiTests.Controllers;

[ApiController]
[Route("[controller]")]
public class OptionsController(
    IOptions<RootOptionSection> rootOptions,
    IOptions<NestedOptionSection> nestedOptions,
    IOptions<TaggedNotLoadedOptions> notLoadedOptions,
    IOptions<UntaggedOptions> untaggedOptions) : ControllerBase
{
    [HttpGet("root")]
    public RootOptionSection GetRoot()
    {
        return rootOptions.Value;
    }

    [HttpGet("nested")]
    public NestedOptionSection GetNested()
    {
        return nestedOptions.Value;
    }

    [HttpGet("notloaded")]
    public TaggedNotLoadedOptions GetNotLoaded()
    {
        return notLoadedOptions.Value;
    }

    [HttpGet("untagged")]
    public UntaggedOptions GetUntagged()
    {
        return untaggedOptions.Value;
    }
}
