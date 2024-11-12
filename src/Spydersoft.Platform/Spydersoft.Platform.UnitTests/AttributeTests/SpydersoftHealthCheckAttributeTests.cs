using Microsoft.Extensions.Diagnostics.HealthChecks;
using Spydersoft.Platform.Attributes;
using Spydersoft.Platform.Exceptions;

namespace Spydersoft.Platform.UnitTests.ExceptionTests
{
    public class SpydersoftHealthCheckAttributeTests
    {
        [Test]
        public void Validate_Constructor()
        {
            var name = "MyHealthCheck";
            var failureStatus = HealthStatus.Unhealthy;
            var tags = "tag1,tag2,,tag3";
            var attribute = new SpydersoftHealthCheckAttribute(name, failureStatus, tags);
            
            Assert.Multiple(() =>
            {
                Assert.That(attribute.Name, Is.EqualTo(name));
                Assert.That(attribute.FailureStatus, Is.EqualTo(failureStatus));
                Assert.That(attribute.RawTags, Is.EqualTo(tags));
                Assert.That(attribute.Tags, Has.Exactly(3).Items);
                Assert.That(attribute.Tags, Has.Exactly(1).Items.EqualTo("tag1"));
                Assert.That(attribute.Tags, Has.Exactly(1).Items.EqualTo("tag2"));
                Assert.That(attribute.Tags, Has.Exactly(1).Items.EqualTo("tag3"));
            });
        }
    }
}
