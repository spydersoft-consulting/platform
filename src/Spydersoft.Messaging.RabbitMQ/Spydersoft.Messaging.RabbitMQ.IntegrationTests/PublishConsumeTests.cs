using Microsoft.Extensions.Hosting;
using Spydersoft.Messaging.RabbitMQ.Options;

namespace Spydersoft.Messaging.RabbitMQ.IntegrationTests;

[NonParallelizable]
internal class PublishConsumeTests
{
    private RabbitMqOptions _options = null!;

    [SetUp]
    public void SetUp()
    {
        _options = TestHelpers.BrokerOptions("test.publish-consume." + Guid.NewGuid().ToString("N"));
        OrderHandler.Reset();
    }

    [Test]
    public async Task PublishAsync_RoundTripsEnvelopeToConsumer()
    {
        var queueName = "test.orders." + Guid.NewGuid().ToString("N");
        using var host = TestHelpers.BuildConsumerHost(_options, b =>
            b.AddConsumer<OrderMessage, OrderHandler>("orders.created", queueName));
        await host.StartAsync();
        await TestHelpers.WaitForQueueAsync(_options, queueName, TimeSpan.FromSeconds(10));

        await using var publisher = TestHelpers.BuildPublisher(_options);
        var envelope = new MessageEnvelope<OrderMessage>
        {
            Payload = new OrderMessage { OrderId = "ORD-42", Amount = 99.95m },
            CorrelationId = "corr-abc",
        };

        await publisher.PublishAsync("orders.created", envelope);

        var received = await OrderHandler.Completion.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Multiple(() =>
        {
            Assert.That(received.Payload.OrderId, Is.EqualTo("ORD-42"));
            Assert.That(received.Payload.Amount, Is.EqualTo(99.95m));
            Assert.That(received.MessageId, Is.EqualTo(envelope.MessageId));
            Assert.That(received.CorrelationId, Is.EqualTo("corr-abc"));
        });

        await host.StopAsync();
    }

    public sealed class OrderMessage
    {
        public string OrderId { get; init; } = string.Empty;
        public decimal Amount { get; init; }
    }

    public sealed class OrderHandler : IMessageHandler<OrderMessage>
    {
        public static TaskCompletionSource<MessageEnvelope<OrderMessage>> Completion { get; private set; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public static void Reset() =>
            Completion = new TaskCompletionSource<MessageEnvelope<OrderMessage>>(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task HandleAsync(MessageEnvelope<OrderMessage> envelope, CancellationToken cancellationToken = default)
        {
            Completion.TrySetResult(envelope);
            return Task.CompletedTask;
        }
    }
}
