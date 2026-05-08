using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Spydersoft.Messaging.RabbitMQ.Options;

namespace Spydersoft.Messaging.RabbitMQ.IntegrationTests;

[NonParallelizable]
internal class AckNackTests
{
    private RabbitMqOptions _options = null!;
    private string _queueName = null!;

    [SetUp]
    public void SetUp()
    {
        _options = TestHelpers.BrokerOptions("test.ack-nack." + Guid.NewGuid().ToString("N"));
        _queueName = "test.queue." + Guid.NewGuid().ToString("N");
        SuccessHandler.Reset();
        FailingHandler.Reset();
    }

    [Test]
    public async Task SuccessfulHandler_AcksMessage_QueueDrains()
    {
        using var host = TestHelpers.BuildConsumerHost(_options, b =>
            b.AddConsumer<Marker, SuccessHandler>("ack.topic", _queueName));
        await host.StartAsync();
        await TestHelpers.WaitForQueueAsync(_options, _queueName, TimeSpan.FromSeconds(10));

        await using var publisher = TestHelpers.BuildPublisher(_options);
        await publisher.PublishAsync("ack.topic", new Marker());

        await SuccessHandler.Completion.Task.WaitAsync(TimeSpan.FromSeconds(10));

        var drained = await TestHelpers.WaitForAsync(
            () => GetQueueMessageCount(_queueName) == 0,
            TimeSpan.FromSeconds(5));

        Assert.That(drained, Is.True, "queue should be empty after successful handler ack");

        await host.StopAsync();
    }

    [Test]
    public async Task HandlerThrows_NacksWithoutRequeue_NoRedelivery()
    {
        using var host = TestHelpers.BuildConsumerHost(_options, b =>
            b.AddConsumer<Marker, FailingHandler>("nack.topic", _queueName));
        await host.StartAsync();
        await TestHelpers.WaitForQueueAsync(_options, _queueName, TimeSpan.FromSeconds(10));

        await using var publisher = TestHelpers.BuildPublisher(_options);
        await publisher.PublishAsync("nack.topic", new Marker());

        await FailingHandler.FirstCall.Task.WaitAsync(TimeSpan.FromSeconds(10));

        // Give the broker time to potentially redeliver — it should not, because nack is requeue=false.
        await Task.Delay(TimeSpan.FromSeconds(2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(FailingHandler.CallCount, Is.EqualTo(1), "handler must not be called more than once");
            Assert.That(GetQueueMessageCount(_queueName), Is.Zero, "queue should be empty (nack discarded message)");
        }

        await host.StopAsync();
    }

    private uint GetQueueMessageCount(string queueName)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
        };
        using var conn = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        using var channel = conn.CreateChannelAsync().GetAwaiter().GetResult();
        var declareOk = channel.QueueDeclarePassiveAsync(queueName).GetAwaiter().GetResult();
        return declareOk.MessageCount;
    }

    public sealed class Marker { }

    public sealed class SuccessHandler : IMessageHandler<Marker>
    {
        public static TaskCompletionSource Completion { get; private set; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public static void Reset() =>
            Completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task HandleAsync(MessageEnvelope<Marker> envelope, CancellationToken cancellationToken = default)
        {
            Completion.TrySetResult();
            return Task.CompletedTask;
        }
    }

    public sealed class FailingHandler : IMessageHandler<Marker>
    {
        private static int _callCount;
        public static int CallCount => Volatile.Read(ref _callCount);
        public static TaskCompletionSource FirstCall { get; private set; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public static void Reset()
        {
            Interlocked.Exchange(ref _callCount, 0);
            FirstCall = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public Task HandleAsync(MessageEnvelope<Marker> envelope, CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _callCount);
            FirstCall.TrySetResult();
            throw new InvalidOperationException("intentional test failure");
        }
    }
}
