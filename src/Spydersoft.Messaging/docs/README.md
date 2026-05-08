# Spydersoft.Messaging

Transport-agnostic messaging abstractions for the Spydersoft Platform.

## Overview

This package provides the core interfaces and contracts for messaging. It has no transport dependency — add a transport package such as `Spydersoft.Messaging.RabbitMQ` to wire up a concrete implementation.

## Core Types

| Type | Description |
|---|---|
| `MessageEnvelope<T>` | Wraps a payload with routing and observability metadata. |
| `IMessagePublisher` | Publishes messages to a named topic. |
| `IMessageHandler<T>` | Implemented by consumers to process a specific message type. |
| `ISpydersoftMessagingBuilder` | Fluent builder returned by transport registration methods. |

## Getting Started

```csharp
// In Program.cs / Startup.cs
services
    .AddSpydersoftMessaging()
    .AddSpydersoftRabbitMq(configuration)
    .AddConsumer<MyMessage, MyMessageHandler>(
        topic: "my.topic",
        queueName: "my-service.my-queue");
```

## Publishing Messages

Inject `IMessagePublisher` and call `PublishAsync`:

```csharp
await publisher.PublishAsync("my.topic", new MyMessage { ... });

// Or supply a pre-built envelope to control metadata:
var envelope = new MessageEnvelope<MyMessage>
{
    Payload = new MyMessage { ... },
    CorrelationId = Activity.Current?.TraceId.ToString()
};
await publisher.PublishAsync("my.topic", envelope);
```

## Consuming Messages

Implement `IMessageHandler<T>` and register it via `AddConsumer`:

```csharp
public sealed class MyMessageHandler : IMessageHandler<MyMessage>
{
    public async Task HandleAsync(MessageEnvelope<MyMessage> envelope, CancellationToken cancellationToken)
    {
        // process envelope.Payload
    }
}
```

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
