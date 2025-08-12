using System;
using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.UnitTests.AttributeTests;

public class DependencyInjectionAttributeTests
{
    [Test]
    public void Validate_Constructor()
    {
        var rank = 10;
        var attribute = new DependencyInjectionAttribute(typeof(SampleService), LifetimeOfService.Singleton, rank);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(attribute.ServiceInterface, Is.EqualTo(typeof(SampleService)));
            Assert.That(attribute.Lifetime, Is.EqualTo(LifetimeOfService.Singleton));
            Assert.That(attribute.Rank, Is.EqualTo(rank));
        }
    }
}

public class SampleService
{

}
