using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Spydersoft.Messaging.RabbitMQ.Options;

namespace Spydersoft.Messaging.RabbitMQ.IntegrationTests;

[NonParallelizable]
internal class BasicPropertiesWireFormatTests
{
    private RabbitMqOptions _options = null!;

    [SetUp]
    public void SetUp() =>
        _options = TestHelpers.BrokerOptions("test.basic-properties." + Guid.NewGuid().ToString("N"));

    [Test]
    public async Task PublishedMessage_HasContentTypeMessageIdAndCorrelationId()
    {
        var queueName = "test.props." + Guid.NewGuid().ToString("N");
        var topic = "props.topic";

        // Set up a raw consumer that captures the BasicDeliverEventArgs (and therefore properties) directly.
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
        };
        await using var conn = await factory.CreateConnectionAsync();
        await using var channel = await conn.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(_options.Exchange, _options.ExchangeType,
            durable: false, autoDelete: false);
        await channel.QueueDeclareAsync(queueName, durable: false, exclusive: false, autoDelete: true);
        await channel.QueueBindAsync(queueName, _options.Exchange, topic);

        var captured = new TaskCompletionSource<BasicDeliverEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, ea) =>
        {
            captured.TrySetResult(ea);
            return Task.CompletedTask;
        };
        await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

        await using var publisher = TestHelpers.BuildPublisher(_options);
        var envelope = new MessageEnvelope<Payload>
        {
            Payload = new Payload { Value = "hello" },
            CorrelationId = "trace-xyz",
        };
        await publisher.PublishAsync(topic, envelope);

        var ea = await captured.Task.WaitAsync(TimeSpan.FromSeconds(10));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ea.BasicProperties.ContentType, Is.EqualTo("application/json"));
            Assert.That(ea.BasicProperties.MessageId, Is.EqualTo(envelope.MessageId));
            Assert.That(ea.BasicProperties.CorrelationId, Is.EqualTo("trace-xyz"));
            var body = Encoding.UTF8.GetString(ea.Body.Span);
            Assert.That(body, Does.Contain("\"hello\""));
        }
    }

    [Test]
    public async Task PublishedMessage_OmitsCorrelationId_WhenEnvelopeHasNone()
    {
        var queueName = "test.props.no-corr." + Guid.NewGuid().ToString("N");
        var topic = "props.no-corr";

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
        };
        await using var conn = await factory.CreateConnectionAsync();
        await using var channel = await conn.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(_options.Exchange, _options.ExchangeType,
            durable: false, autoDelete: false);
        await channel.QueueDeclareAsync(queueName, durable: false, exclusive: false, autoDelete: true);
        await channel.QueueBindAsync(queueName, _options.Exchange, topic);

        var captured = new TaskCompletionSource<BasicDeliverEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, ea) =>
        {
            captured.TrySetResult(ea);
            return Task.CompletedTask;
        };
        await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

        await using var publisher = TestHelpers.BuildPublisher(_options);
        await publisher.PublishAsync(topic, new Payload { Value = "no-corr" });

        var ea = await captured.Task.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.That(ea.BasicProperties.CorrelationId, Is.Null.Or.Empty);
    }

    public sealed class Payload
    {
        public string Value { get; init; } = string.Empty;
    }
}
