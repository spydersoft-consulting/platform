using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.OptionsTests;

[SpydersoftOptions(nameof(TaggedNotLoadedOptions), "notloaded")]
public class TaggedNotLoadedOptions
{
    public string NotLoadedOption1 { get; set; } = "NotLoadedOption1";
}
