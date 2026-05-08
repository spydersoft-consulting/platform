namespace Spydersoft.Messaging.RabbitMQ;

public sealed record RabbitMqConsumerRegistration(
    string Topic,
    string QueueName,
    Type MessageType,
    Type HandlerType
);
