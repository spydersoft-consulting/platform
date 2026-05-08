using Microsoft.Extensions.DependencyInjection;

namespace Spydersoft.Messaging;

public interface ISpydersoftMessagingBuilder
{
    IServiceCollection Services { get; }

    /// <summary>
    /// Registers a typed message handler for the given topic.
    /// A dedicated queue named <paramref name="queueName"/> is declared and bound to the topic.
    /// </summary>
    ISpydersoftMessagingBuilder AddConsumer<TMessage, THandler>(string topic, string queueName)
        where THandler : class, IMessageHandler<TMessage>;
}
