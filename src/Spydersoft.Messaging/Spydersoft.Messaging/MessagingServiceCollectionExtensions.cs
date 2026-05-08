using Microsoft.Extensions.DependencyInjection;

namespace Spydersoft.Messaging;

public static class MessagingServiceCollectionExtensions
{
    /// <summary>
    /// Registers Spydersoft.Messaging core services.
    /// Call this before adding a transport implementation (e.g. AddSpydersoftRabbitMq).
    /// </summary>
    public static IServiceCollection AddSpydersoftMessaging(this IServiceCollection services)
    {
        // No-op placeholder; provides a consistent registration entry point
        // and a place to add cross-cutting concerns (e.g. middleware pipeline, metrics) later.
        return services;
    }
}
