# Spydersoft.Messaging — Specification

## Overview

Add a transport-agnostic messaging abstraction to the Spydersoft Platform, distributed as two NuGet packages:

| Package | Purpose |
|---|---|
| `Spydersoft.Messaging` | Core interfaces and contracts. No transport dependency. |
| `Spydersoft.Messaging.RabbitMQ` | RabbitMQ.Client implementation of the core interfaces. |

The split follows the same pattern already used in `Spydersoft.Platform` / `Spydersoft.Platform.Hosting`: a transport-agnostic core that consumers depend on, and a concrete implementation that can be swapped (e.g., for Kafka) later by providing a new package without changing callers.

---

## Repository Location

New solution folder inside the existing platform repo:

```
src/
  Spydersoft.Messaging/
    Spydersoft.Messaging/               ← core library
    Spydersoft.Messaging.UnitTests/
    docs/
      README.md
  Spydersoft.Messaging.RabbitMQ/
    Spydersoft.Messaging.RabbitMQ/      ← RabbitMQ implementation
    Spydersoft.Messaging.RabbitMQ.IntegrationTests/
    docs/
      README.md
```

---

## Target Frameworks

Both packages should multi-target to match the rest of the platform:

```xml
<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>
```

---

## Package: `Spydersoft.Messaging`

### Namespace root
`Spydersoft.Messaging`

### Core Types

#### `MessageEnvelope<T>`

Wraps a payload with routing and observability metadata. This is the over-the-wire type — both publisher and consumer deal in envelopes.

```csharp
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
```

#### `IMessagePublisher`

```csharp
namespace Spydersoft.Messaging;

public interface IMessagePublisher
{
    /// <summary>
    /// Publishes a message to the specified topic.
    /// The message is wrapped in a MessageEnvelope by the implementation.
    /// </summary>
    Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a pre-built envelope to the specified topic.
    /// Use this overload when you need to control envelope metadata (e.g. CorrelationId).
    /// </summary>
    Task PublishAsync<T>(string topic, MessageEnvelope<T> envelope, CancellationToken cancellationToken = default);
}
```

#### `IMessageHandler<T>`

Implemented by consumers to process a specific message type.

```csharp
namespace Spydersoft.Messaging;

public interface IMessageHandler<T>
{
    Task HandleAsync(MessageEnvelope<T> envelope, CancellationToken cancellationToken = default);
}
```

### DI Registration

Extension method on `IServiceCollection` (not `WebApplicationBuilder`, since this core package should work in non-web host scenarios like worker services):

```csharp
namespace Spydersoft.Messaging;

public static class MessagingServiceCollectionExtensions
{
    /// <summary>
    /// Registers Spydersoft.Messaging core services.
    /// Call this before adding a transport implementation (e.g. AddSpydersoftRabbitMq).
    /// </summary>
    public static IServiceCollection AddSpydersoftMessaging(this IServiceCollection services)
    {
        // Currently a no-op placeholder; provides a consistent registration entry point
        // and a place to add cross-cutting concerns (e.g. middleware pipeline, metrics) later.
        return services;
    }
}
```

### Notes

- No transport packages referenced — `Spydersoft.Messaging` depends only on `Microsoft.Extensions.DependencyInjection.Abstractions`.
- `MessageEnvelope<T>` is serialized as JSON over the wire; implementations are responsible for serialization. Use `System.Text.Json` as the default.

---

## Package: `Spydersoft.Messaging.RabbitMQ`

### Namespace root
`Spydersoft.Messaging.RabbitMQ`

### NuGet Dependencies

```xml
<PackageReference Include="RabbitMQ.Client" />           <!-- Apache 2.0 -->
<PackageReference Include="Spydersoft.Messaging" />
<PackageReference Include="Microsoft.Extensions.Hosting.Abstractions" />
<PackageReference Include="Microsoft.Extensions.Options" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" />
```

### Configuration

#### `RabbitMqOptions`

```csharp
namespace Spydersoft.Messaging.RabbitMQ.Options;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Exchange to publish messages to. Defaults to a direct exchange.
    /// </summary>
    public string Exchange { get; set; } = "spydersoft.messaging";

    /// <summary>
    /// Exchange type: "direct", "topic", "fanout", "headers".
    /// </summary>
    public string ExchangeType { get; set; } = "topic";

    /// <summary>
    /// Whether the exchange should survive broker restarts.
    /// </summary>
    public bool DurableExchange { get; set; } = true;

    /// <summary>
    /// Whether published messages are persisted to disk.
    /// </summary>
    public bool PersistentMessages { get; set; } = true;
}
```

appsettings.json example:
```json
{
  "RabbitMq": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "Exchange": "spydersoft.messaging",
    "ExchangeType": "topic"
  }
}
```

### `RabbitMqMessagePublisher`

Implements `IMessagePublisher`. Registered as a **singleton**.

Behaviour:
- Creates a single `IConnection` on first use (lazy, thread-safe).
- Opens a new `IChannel` per `PublishAsync` call (channels are not thread-safe in RabbitMQ.Client v7+).
- Serializes the `MessageEnvelope<T>` to JSON using `System.Text.Json`.
- Sets `BasicProperties.DeliveryMode` to persistent when `RabbitMqOptions.PersistentMessages` is true.
- Uses the topic as the routing key.

```csharp
namespace Spydersoft.Messaging.RabbitMQ;

public sealed class RabbitMqMessagePublisher : IMessagePublisher, IAsyncDisposable
{
    // Constructor: IOptions<RabbitMqOptions>, ILogger<RabbitMqMessagePublisher>
    // ...
}
```

### Consumer Registration

Rather than a single `IMessageConsumer` interface (which would require consumers to deal with deserialization and routing themselves), the RabbitMQ package uses a **typed registration** model. Callers register `IMessageHandler<T>` implementations with a topic binding; the package wires up a background service that routes inbound messages to the correct handler.

#### `RabbitMqConsumerRegistration`

Internal record holding the binding for one handler:

```csharp
internal sealed record RabbitMqConsumerRegistration(
    string Topic,
    string QueueName,
    Type MessageType,
    Type HandlerType
);
```

#### `RabbitMqConsumerBackgroundService`

A `BackgroundService` that:
1. Creates one connection and one channel per registered queue on startup.
2. Declares the queue and binds it to the exchange with the configured routing key/topic.
3. Starts a basic consumer loop.
4. Deserializes inbound messages to `MessageEnvelope<T>` using `System.Text.Json`.
5. Resolves the appropriate `IMessageHandler<T>` from a scoped `IServiceProvider` (one scope per message).
6. Calls `handler.HandleAsync(envelope, ct)`.
7. Acks the message on success; nacks (without requeue) on unhandled exception, sending the message to the dead-letter exchange if configured.

### DI Registration

```csharp
namespace Spydersoft.Messaging.RabbitMQ;

public static class RabbitMqMessagingBuilderExtensions
{
    /// <summary>
    /// Adds the RabbitMQ transport for Spydersoft.Messaging.
    /// </summary>
    public static ISpydersoftMessagingBuilder AddSpydersoftRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();
        services.AddHostedService<RabbitMqConsumerBackgroundService>();
        return new SpydersoftMessagingBuilder(services);
    }

    /// <summary>
    /// Overload accepting an action for inline configuration.
    /// </summary>
    public static ISpydersoftMessagingBuilder AddSpydersoftRabbitMq(
        this IServiceCollection services,
        Action<RabbitMqOptions> configure)
    { ... }
}
```

#### `ISpydersoftMessagingBuilder` / `SpydersoftMessagingBuilder`

A builder returned from `AddSpydersoftRabbitMq` that allows fluent consumer registration:

```csharp
namespace Spydersoft.Messaging;

public interface ISpydersoftMessagingBuilder
{
    IServiceCollection Services { get; }

    /// <summary>
    /// Registers a typed message handler for the given topic.
    /// A dedicated queue named <paramref name="queueName"/> is declared and bound to the topic.
    /// </summary>
    ISpydersoftMessagingBuilder AddConsumer<TMessage, THandler>(
        string topic,
        string queueName)
        where THandler : class, IMessageHandler<TMessage>;
}
```

Usage example:
```csharp
services
    .AddSpydersoftMessaging()
    .AddSpydersoftRabbitMq(configuration)
    .AddConsumer<AuditEvent, AuditEventHandler>(
        topic: "audit.events",
        queueName: "pitstop.audit-processor");
```

`AddConsumer` should:
- Register `THandler` as `IMessageHandler<TMessage>` (scoped lifetime).
- Store a `RabbitMqConsumerRegistration` in the DI container (e.g. as `IEnumerable<RabbitMqConsumerRegistration>`) so `RabbitMqConsumerBackgroundService` can discover all bindings.

---

## How PitStop Uses These Packages

This section is informational — it lives in PitStop, not the platform repo.

### Audit.NET Data Provider

In `Spydersoft.PitStop.Data`, implement a custom Audit.NET provider:

```csharp
public sealed class RabbitMqAuditDataProvider : AuditDataProvider
{
    // Constructor: IMessagePublisher
    // Topic constant: "audit.events"

    public override object InsertEvent(AuditEvent auditEvent)
    {
        // fire-and-forget with Task.Run or use a channel for backpressure
        _publisher.PublishAsync("audit.events", auditEvent).GetAwaiter().GetResult();
        return auditEvent.EventId ?? Guid.NewGuid().ToString();
    }

    public override async Task<object> InsertEventAsync(AuditEvent auditEvent, CancellationToken ct)
    {
        await _publisher.PublishAsync("audit.events", auditEvent, ct);
        return auditEvent.EventId ?? Guid.NewGuid().ToString();
    }
}
```

Configure Audit.NET in `Program.cs`:
```csharp
Audit.Core.Configuration.Setup()
    .UseCustomProvider(new RabbitMqAuditDataProvider(publisher));
```

### AuditProcessor Worker Service

New project: `Spydersoft.PitStop.AuditProcessor` (.NET Worker Service).

Implements `IMessageHandler<AuditEvent>`:
```csharp
public sealed class AuditEventHandler : IMessageHandler<AuditEvent>
{
    // Constructor: IMongoCollection<AuditEventDocument>, ILogger<...>

    public async Task HandleAsync(MessageEnvelope<AuditEvent> envelope, CancellationToken ct)
    {
        var doc = Map(envelope);
        await _collection.InsertOneAsync(doc, cancellationToken: ct);
    }
}
```

MongoDB document model:
```csharp
public sealed class AuditEventDocument
{
    [BsonId] public ObjectId Id { get; init; }
    public string EventType { get; init; } = default!;
    public string EntityType { get; init; } = default!;
    public string EntityId { get; init; } = default!;
    public string Action { get; init; } = default!;     // Insert / Update / Delete
    public string? UserId { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public BsonDocument? OldValues { get; init; }
    public BsonDocument? NewValues { get; init; }
    public string? CorrelationId { get; init; }
}
```

---

## Aspire AppHost Changes (PitStop)

Add RabbitMQ and MongoDB as Aspire resources:

```csharp
var rabbitmq = builder.AddRabbitMQ("rabbitmq");
var mongo = builder.AddMongoDB("mongo");

builder.AddProject<Projects.Spydersoft_PitStop_Api>("api")
    .WithReference(rabbitmq)
    .WithReference(postgres);

builder.AddProject<Projects.Spydersoft_PitStop_AuditProcessor>("audit-processor")
    .WithReference(rabbitmq)
    .WithReference(mongo);
```

Aspire packages needed in AppHost:
- `Aspire.Hosting.RabbitMQ`
- `Aspire.Hosting.MongoDB`

---

## Out of Scope (for now)

- Dead-letter exchange / dead-letter queue configuration (noted but not implemented in v1).
- Kafka transport (`Spydersoft.Messaging.Kafka` — future package).
- Message schema registry / versioning.
- Publisher confirms / outbox pattern (considered; deferred to future iteration).
- Retry policies on the consumer (consider adding Polly later).
