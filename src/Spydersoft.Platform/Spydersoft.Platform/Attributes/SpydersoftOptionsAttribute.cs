using System;

namespace Spydersoft.Platform.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SpydersoftOptionsAttribute : Attribute
    {
        public SpydersoftOptionsAttribute(string sectionName, string tags = "")
        {
            SectionName = sectionName;
            RawTags = tags;
            Tags = tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
        public string SectionName { get; }

        public string RawTags { get; }

        public string[] Tags { get; }
    }
}
