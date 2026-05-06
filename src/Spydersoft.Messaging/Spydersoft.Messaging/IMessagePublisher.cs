namespace Spydersoft.Messaging;

public interface IMessagePublisher
{
    /// <summary>
    /// Publishes a message to the specified topic.
    /// The message is wrapped in a <see cref="MessageEnvelope{T}"/> by the implementation.
    /// </summary>
    Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a pre-built envelope to the specified topic.
    /// Use this overload when you need to control envelope metadata (e.g. CorrelationId).
    /// </summary>
    Task PublishAsync<T>(string topic, MessageEnvelope<T> envelope, CancellationToken cancellationToken = default);
}
