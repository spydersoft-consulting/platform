namespace Spydersoft.Messaging;

public interface IMessageHandler<T>
{
    Task HandleAsync(MessageEnvelope<T> envelope, CancellationToken cancellationToken = default);
}
