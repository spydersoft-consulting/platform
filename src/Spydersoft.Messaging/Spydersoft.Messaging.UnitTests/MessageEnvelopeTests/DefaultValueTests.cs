using Spydersoft.Messaging;

namespace Spydersoft.Messaging.UnitTests.MessageEnvelopeTests;

internal class DefaultValueTests
{
    [Test]
    public void MessageId_IsValidNonEmptyGuid()
    {
        var envelope = new MessageEnvelope<string> { Payload = "test" };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(envelope.MessageId, Is.Not.Null.And.Not.Empty);
            Assert.That(Guid.TryParse(envelope.MessageId, out _), Is.True);
        }
    }

    [Test]
    public void PublishedAt_IsUtc()
    {
        var before = DateTimeOffset.UtcNow;
        var envelope = new MessageEnvelope<string> { Payload = "test" };
        var after = DateTimeOffset.UtcNow;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(envelope.PublishedAt.Offset, Is.EqualTo(TimeSpan.Zero));
            Assert.That(envelope.PublishedAt, Is.GreaterThanOrEqualTo(before));
            Assert.That(envelope.PublishedAt, Is.LessThanOrEqualTo(after));
        }
    }

    [Test]
    public void OptionalFields_DefaultToNull()
    {
        var envelope = new MessageEnvelope<string> { Payload = "test" };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(envelope.CorrelationId, Is.Null);
            Assert.That(envelope.Source, Is.Null);
            Assert.That(envelope.Topic, Is.Null);
        }
    }

    [Test]
    public void EachInstance_HasUniqueMessageId()
    {
        var first = new MessageEnvelope<int> { Payload = 1 };
        var second = new MessageEnvelope<int> { Payload = 2 };

        Assert.That(first.MessageId, Is.Not.EqualTo(second.MessageId));
    }

    [Test]
    public void Payload_IsPreserved()
    {
        var payload = new { Value = 42 };
        var envelope = new MessageEnvelope<object> { Payload = payload };

        Assert.That(envelope.Payload, Is.SameAs(payload));
    }
}
