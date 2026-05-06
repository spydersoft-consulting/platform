using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Spydersoft.Messaging.RabbitMQ.Options;

namespace Spydersoft.Messaging.RabbitMQ;

public sealed class RabbitMqMessagePublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqMessagePublisher> _logger;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;

    public RabbitMqMessagePublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqMessagePublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default)
    {
        var envelope = new MessageEnvelope<T> { Payload = message, Topic = topic };
        return PublishAsync(topic, envelope, cancellationToken);
    }

    public async Task PublishAsync<T>(string topic, MessageEnvelope<T> envelope, CancellationToken cancellationToken = default)
    {
        var connection = await GetOrCreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: _options.Exchange,
            type: _options.ExchangeType,
            durable: _options.DurableExchange,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(envelope);

        var properties = new BasicProperties
        {
            DeliveryMode = _options.PersistentMessages ? DeliveryModes.Persistent : DeliveryModes.Transient
        };

        await channel.BasicPublishAsync(
            exchange: _options.Exchange,
            routingKey: topic,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Published message to topic {Topic} on exchange {Exchange}", topic, _options.Exchange);
    }

    private async Task<IConnection> GetOrCreateConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                VirtualHost = _options.VirtualHost,
                UserName = _options.Username,
                Password = _options.Password
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _logger.LogInformation("RabbitMQ connection established to {Host}:{Port}", _options.Host, _options.Port);
            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }
        _connectionLock.Dispose();
    }
}
