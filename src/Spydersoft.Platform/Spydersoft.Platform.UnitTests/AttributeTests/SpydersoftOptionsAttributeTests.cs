using System;
using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.UnitTests.AttributeTests;

public class SpydersoftOptionsAttributeTests
{
 [Test]
    public void Validate_Constructor()
    {
        var name = "MySectionName";
        var tags = "tag1,tag2,,tag3";
        var attribute = new SpydersoftOptionsAttribute(name, tags);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(attribute.SectionName, Is.EqualTo(name));
            Assert.That(attribute.RawTags, Is.EqualTo(tags));
            Assert.That(attribute.Tags, Has.Exactly(3).Items);
            Assert.That(attribute.Tags, Has.Exactly(1).Items.EqualTo("tag1"));
            Assert.That(attribute.Tags, Has.Exactly(1).Items.EqualTo("tag2"));
            Assert.That(attribute.Tags, Has.Exactly(1).Items.EqualTo("tag3"));
        }
    }
}
