using Microsoft.Extensions.DependencyInjection;

namespace Spydersoft.Messaging.RabbitMQ;

internal sealed class SpydersoftMessagingBuilder : ISpydersoftMessagingBuilder
{
    public IServiceCollection Services { get; }

    public SpydersoftMessagingBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public ISpydersoftMessagingBuilder AddConsumer<TMessage, THandler>(string topic, string queueName)
        where THandler : class, IMessageHandler<TMessage>
    {
        Services.AddScoped<IMessageHandler<TMessage>, THandler>();
        Services.AddSingleton(new RabbitMqConsumerRegistration(
            Topic: topic,
            QueueName: queueName,
            MessageType: typeof(TMessage),
            HandlerType: typeof(THandler)));

        return this;
    }
}
