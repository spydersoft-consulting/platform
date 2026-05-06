namespace Spydersoft.Messaging;

public sealed class MessageEnvelope<T>
{
    public required T Payload { get; init; }
    public string MessageId { get; init; } = Guid.NewGuid().ToString();
    public DateTimeOffset PublishedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? CorrelationId { get; init; }
    public string? Source { get; init; }
    public string? Topic { get; init; }
}
