using System.Text.Json;

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
    /// Exchange to publish messages to.
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

    /// <summary>
    /// JSON serializer options used for envelope serialization. Defaults to
    /// <see cref="JsonSerializerDefaults.Web"/> (camelCase property names). Both
    /// publisher and consumer use the same instance, so the round-trip stays consistent.
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; set; }
        = new(JsonSerializerDefaults.Web);
}
