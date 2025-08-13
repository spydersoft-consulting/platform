using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.OptionsTests;

[InjectOptions(nameof(RootOptionSection), "root")]
public class RootOptionSection
{
    public string Option1 { get; set; } = "Option1";

    public string Option2 { get; set; } = "Option2";
}
