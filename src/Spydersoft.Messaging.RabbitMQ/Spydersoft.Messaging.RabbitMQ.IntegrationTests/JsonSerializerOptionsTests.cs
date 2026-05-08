using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Spydersoft.Messaging.RabbitMQ.Options;

namespace Spydersoft.Messaging.RabbitMQ.IntegrationTests;

[NonParallelizable]
internal class JsonSerializerOptionsTests
{
    [Test]
    public async Task CustomJsonSerializerOptions_AreUsedByPublisherAndConsumer_RoundTrip()
    {
        // SnakeCaseLower is naming policy that survives a real round-trip; the default Web policy is camelCase.
        var customOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = false,
        };

        var options = TestHelpers.BrokerOptions(
            "test.json-options." + Guid.NewGuid().ToString("N"),
            o => o.JsonSerializerOptions = customOptions);

        SnakeHandler.Reset();

        var queueName = "test.snake." + Guid.NewGuid().ToString("N");
        using var host = TestHelpers.BuildConsumerHost(options, b =>
            b.AddConsumer<SnakeMessage, SnakeHandler>("snake.topic", queueName));
        await host.StartAsync();
        await TestHelpers.WaitForQueueAsync(options, queueName, TimeSpan.FromSeconds(10));

        await using var publisher = TestHelpers.BuildPublisher(options);
        await publisher.PublishAsync("snake.topic", new SnakeMessage { LongPropertyName = "round-trip-ok" });

        var received = await SnakeHandler.Completion.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.That(received.Payload.LongPropertyName, Is.EqualTo("round-trip-ok"));

        await host.StopAsync();
    }

    [Test]
    public async Task CustomJsonSerializerOptions_AreReflectedOnTheWire()
    {
        var customOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        var options = TestHelpers.BrokerOptions(
            "test.json-wire." + Guid.NewGuid().ToString("N"),
            o => o.JsonSerializerOptions = customOptions);

        var queueName = "test.snake-wire." + Guid.NewGuid().ToString("N");
        var topic = "snake.wire";

        var factory = new ConnectionFactory
        {
            HostName = options.Host,
            Port = options.Port,
            UserName = options.Username,
            Password = options.Password,
        };
        await using var conn = await factory.CreateConnectionAsync();
        await using var channel = await conn.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(options.Exchange, options.ExchangeType,
            durable: false, autoDelete: false);
        await channel.QueueDeclareAsync(queueName, durable: false, exclusive: false, autoDelete: true);
        await channel.QueueBindAsync(queueName, options.Exchange, topic);

        var captured = new TaskCompletionSource<BasicDeliverEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, ea) =>
        {
            captured.TrySetResult(ea);
            return Task.CompletedTask;
        };
        await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

        await using var publisher = TestHelpers.BuildPublisher(options);
        await publisher.PublishAsync(topic, new SnakeMessage { LongPropertyName = "wire" });

        var ea = await captured.Task.WaitAsync(TimeSpan.FromSeconds(10));
        var body = Encoding.UTF8.GetString(ea.Body.Span);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(body, Does.Contain("long_property_name"), "expected snake_case property name from custom JSON options");
            Assert.That(body, Does.Not.Contain("longPropertyName"));
        }
    }

    public sealed class SnakeMessage
    {
        public string LongPropertyName { get; init; } = string.Empty;
    }

    public sealed class SnakeHandler : IMessageHandler<SnakeMessage>
    {
        public static TaskCompletionSource<MessageEnvelope<SnakeMessage>> Completion { get; private set; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public static void Reset() =>
            Completion = new TaskCompletionSource<MessageEnvelope<SnakeMessage>>(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task HandleAsync(MessageEnvelope<SnakeMessage> envelope, CancellationToken cancellationToken = default)
        {
            Completion.TrySetResult(envelope);
            return Task.CompletedTask;
        }
    }
}
