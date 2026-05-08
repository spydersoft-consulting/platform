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

## Failure Semantics

- **Publishing** (`IMessagePublisher.PublishAsync`): throws on transport failure. Callers
  that need fire-and-forget semantics (e.g. logging audit events) must wrap the call in
  their own try/catch.
- **Consuming** (`IMessageHandler<T>.HandleAsync`): an unhandled exception causes the
  underlying transport to nack the message without requeue, routing to the dead-letter
  exchange if the broker has one configured.
- **Cancellation**: handlers should respect `CancellationToken` and propagate
  `OperationCanceledException` on shutdown — the transport will release in-flight messages
  back to the queue.

## JSON Serialization

Envelopes are serialized as JSON using `JsonSerializerDefaults.Web` by default (camelCase
property names). To customize, set `RabbitMqOptions.JsonSerializerOptions` — both publisher
and consumer use the same instance, so the round-trip stays consistent.
