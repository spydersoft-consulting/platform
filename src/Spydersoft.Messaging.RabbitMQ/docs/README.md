# Spydersoft.Messaging.RabbitMQ

RabbitMQ transport implementation for `Spydersoft.Messaging`.

## Overview

This package wires up `IMessagePublisher` and a hosted consumer service backed by [RabbitMQ.Client](https://www.nuget.org/packages/RabbitMQ.Client). It requires `Spydersoft.Messaging` for the core interfaces.

## Configuration

Add a `RabbitMq` section to `appsettings.json`:

```json
{
  "RabbitMq": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "Exchange": "spydersoft.messaging",
    "ExchangeType": "topic",
    "DurableExchange": true,
    "PersistentMessages": true
  }
}
```

## Registration

```csharp
services
    .AddSpydersoftMessaging()
    .AddSpydersoftRabbitMq(configuration)
    .AddConsumer<MyMessage, MyMessageHandler>(
        topic: "my.topic",
        queueName: "my-service.my-queue");
```

Or configure inline:

```csharp
services
    .AddSpydersoftMessaging()
    .AddSpydersoftRabbitMq(opts =>
    {
        opts.Host = "rabbitmq";
        opts.Exchange = "my-exchange";
    });
```

## Publishing

Inject `IMessagePublisher`:

```csharp
await publisher.PublishAsync("my.topic", new MyMessage { ... });
```

## Consuming

Implement `IMessageHandler<T>` and register with `AddConsumer`. The background service handles connection lifecycle, deserialization, ack/nack, and scoped handler resolution automatically.

```csharp
public sealed class MyMessageHandler : IMessageHandler<MyMessage>
{
    public async Task HandleAsync(MessageEnvelope<MyMessage> envelope, CancellationToken cancellationToken)
    {
        // process envelope.Payload
    }
}
```

Unhandled exceptions result in a nack without requeue, routing the message to the dead-letter exchange if configured on the broker.
