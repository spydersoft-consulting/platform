using Microsoft.Extensions.DependencyInjection;
using Spydersoft.Messaging;

namespace Spydersoft.Messaging.UnitTests.MessagingServiceCollectionExtensionsTests;

internal class AddSpydersoftMessagingTests
{
    [Test]
    public void AddSpydersoftMessaging_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddSpydersoftMessaging();

        Assert.That(result, Is.SameAs(services));
    }

    [Test]
    public void AddSpydersoftMessaging_DoesNotThrow()
    {
        var services = new ServiceCollection();

        Assert.DoesNotThrow(() => services.AddSpydersoftMessaging());
    }
}
