using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spydersoft.Messaging.RabbitMQ.Options;

namespace Spydersoft.Messaging.RabbitMQ;

public static class RabbitMqMessagingBuilderExtensions
{
    /// <summary>
    /// Adds the RabbitMQ transport for Spydersoft.Messaging, binding options from configuration.
    /// </summary>
    public static ISpydersoftMessagingBuilder AddSpydersoftRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        return RegisterCommon(services);
    }

    /// <summary>
    /// Adds the RabbitMQ transport for Spydersoft.Messaging with inline options configuration.
    /// </summary>
    public static ISpydersoftMessagingBuilder AddSpydersoftRabbitMq(
        this IServiceCollection services,
        Action<RabbitMqOptions> configure)
    {
        services.Configure(configure);
        return RegisterCommon(services);
    }

    private static ISpydersoftMessagingBuilder RegisterCommon(IServiceCollection services)
    {
        services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();
        services.AddHostedService<RabbitMqConsumerBackgroundService>();
        return new SpydersoftMessagingBuilder(services);
    }
}
