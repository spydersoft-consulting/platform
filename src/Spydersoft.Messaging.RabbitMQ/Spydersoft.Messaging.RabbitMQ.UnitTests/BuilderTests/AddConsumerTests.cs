using Microsoft.Extensions.DependencyInjection;
using Spydersoft.Messaging;
using Spydersoft.Messaging.RabbitMQ;

namespace Spydersoft.Messaging.RabbitMQ.UnitTests.BuilderTests;

internal class AddConsumerTests
{
    private IServiceCollection _services = null!;
    private ISpydersoftMessagingBuilder _builder = null!;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _builder = _services.AddSpydersoftRabbitMq(opts => opts.Host = "localhost");
    }

    [Test]
    public void AddConsumer_RegistersHandlerAsScoped()
    {
        _builder.AddConsumer<TestMessage, TestMessageHandler>("test.topic", "test-queue");

        var descriptor = _services.FirstOrDefault(d =>
            d.ServiceType == typeof(IMessageHandler<TestMessage>) &&
            d.ImplementationType == typeof(TestMessageHandler));

        Assert.That(descriptor, Is.Not.Null);
        Assert.That(descriptor!.Lifetime, Is.EqualTo(ServiceLifetime.Scoped));
    }

    [Test]
    public void AddConsumer_RegistersConsumerRegistrationWithCorrectValues()
    {
        _builder.AddConsumer<TestMessage, TestMessageHandler>("test.topic", "test-queue");

        var provider = _services.BuildServiceProvider();
        var registrations = provider.GetServices<RabbitMqConsumerRegistration>().ToList();

        Assert.That(registrations, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(registrations[0].Topic, Is.EqualTo("test.topic"));
            Assert.That(registrations[0].QueueName, Is.EqualTo("test-queue"));
            Assert.That(registrations[0].MessageType, Is.EqualTo(typeof(TestMessage)));
            Assert.That(registrations[0].HandlerType, Is.EqualTo(typeof(TestMessageHandler)));
        }
    }

    [Test]
    public void AddConsumer_ReturnsBuilderForFluentChaining()
    {
        var result = _builder.AddConsumer<TestMessage, TestMessageHandler>("test.topic", "test-queue");

        Assert.That(result, Is.SameAs(_builder));
    }

    [Test]
    public void AddConsumer_MultipleConsumers_RegistersAll()
    {
        _builder
            .AddConsumer<TestMessage, TestMessageHandler>("topic.one", "queue-one")
            .AddConsumer<AnotherMessage, AnotherMessageHandler>("topic.two", "queue-two");

        var provider = _services.BuildServiceProvider();
        var registrations = provider.GetServices<RabbitMqConsumerRegistration>().ToList();

        Assert.That(registrations, Has.Count.EqualTo(2));
    }

    [Test]
    public void AddSpydersoftRabbitMq_RegistersMessagePublisherAsSingleton()
    {
        var descriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IMessagePublisher));

        Assert.That(descriptor, Is.Not.Null);
        Assert.That(descriptor!.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
    }

    [Test]
    public void AddSpydersoftRabbitMq_RegistersConsumerBackgroundService()
    {
        var descriptor = _services.FirstOrDefault(d =>
            d.ImplementationType == typeof(RabbitMqConsumerBackgroundService));

        Assert.That(descriptor, Is.Not.Null);
    }

    [Test]
    public void AddSpydersoftRabbitMq_ReturnsBuilder()
    {
        Assert.That(_builder, Is.Not.Null);
        Assert.That(_builder.Services, Is.SameAs(_services));
    }

    private sealed class TestMessage { }
    private sealed class AnotherMessage { }

    private sealed class TestMessageHandler : IMessageHandler<TestMessage>
    {
        public Task HandleAsync(MessageEnvelope<TestMessage> envelope, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class AnotherMessageHandler : IMessageHandler<AnotherMessage>
    {
        public Task HandleAsync(MessageEnvelope<AnotherMessage> envelope, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
