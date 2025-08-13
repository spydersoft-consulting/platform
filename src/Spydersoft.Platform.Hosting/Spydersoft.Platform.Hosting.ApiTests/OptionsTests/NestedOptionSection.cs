using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.OptionsTests;

[InjectOptions(nameof(NestedOptionSection), "nested")]
public class NestedOptionSection
{
    public string NestedOption1 { get; set; } = "NestedOption1";

    public string NestedOption2 { get; set; } = "NestedOption2";
}
