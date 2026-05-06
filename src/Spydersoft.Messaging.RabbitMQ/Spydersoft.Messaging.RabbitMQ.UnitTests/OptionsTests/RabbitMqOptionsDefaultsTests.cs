using Spydersoft.Messaging.RabbitMQ.Options;

namespace Spydersoft.Messaging.RabbitMQ.UnitTests.OptionsTests;

internal class RabbitMqOptionsDefaultsTests
{
    private RabbitMqOptions _options = null!;

    [SetUp]
    public void SetUp() => _options = new RabbitMqOptions();

    [Test]
    public void SectionName_IsRabbitMq() =>
        Assert.That(RabbitMqOptions.SectionName, Is.EqualTo("RabbitMq"));

    [Test]
    public void Host_DefaultsToLocalhost() =>
        Assert.That(_options.Host, Is.EqualTo("localhost"));

    [Test]
    public void Port_DefaultsTo5672() =>
        Assert.That(_options.Port, Is.EqualTo(5672));

    [Test]
    public void VirtualHost_DefaultsToSlash() =>
        Assert.That(_options.VirtualHost, Is.EqualTo("/"));

    [Test]
    public void Username_DefaultsToGuest() =>
        Assert.That(_options.Username, Is.EqualTo("guest"));

    [Test]
    public void Password_DefaultsToGuest() =>
        Assert.That(_options.Password, Is.EqualTo("guest"));

    [Test]
    public void Exchange_DefaultsToSpydersoftMessaging() =>
        Assert.That(_options.Exchange, Is.EqualTo("spydersoft.messaging"));

    [Test]
    public void ExchangeType_DefaultsToTopic() =>
        Assert.That(_options.ExchangeType, Is.EqualTo("topic"));

    [Test]
    public void DurableExchange_DefaultsToTrue() =>
        Assert.That(_options.DurableExchange, Is.True);

    [Test]
    public void PersistentMessages_DefaultsToTrue() =>
        Assert.That(_options.PersistentMessages, Is.True);
}
