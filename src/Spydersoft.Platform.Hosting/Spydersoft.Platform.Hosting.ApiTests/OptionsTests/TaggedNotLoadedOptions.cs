using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.OptionsTests;

[InjectOptions(nameof(TaggedNotLoadedOptions), "notloaded")]
public class TaggedNotLoadedOptions
{
    public string NotLoadedOption1 { get; set; } = "NotLoadedOption1";
}
