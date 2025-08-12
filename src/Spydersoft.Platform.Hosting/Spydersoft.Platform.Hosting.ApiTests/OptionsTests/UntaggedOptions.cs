using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.OptionsTests;

[InjectOptions(nameof(UntaggedOptions))]
public class UntaggedOptions
{
    public string Untagged1 { get; set; } = "Untagged1";
}
