using System;

namespace Spydersoft.Platform.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class InjectOptionsAttribute : Attribute
{
    public InjectOptionsAttribute(string sectionName, string tags = "")
    {
        SectionName = sectionName;
        RawTags = tags;
        Tags = tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }
    public string SectionName { get; }

    public string RawTags { get; }

    public string[] Tags { get; }
}
