using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Spydersoft.Messaging.RabbitMQ.Options;

namespace Spydersoft.Messaging.RabbitMQ;

public sealed class RabbitMqConsumerBackgroundService : BackgroundService
{
    private readonly IEnumerable<RabbitMqConsumerRegistration> _registrations;
    private readonly RabbitMqOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqConsumerBackgroundService> _logger;

    private IConnection? _connection;
    private readonly List<IChannel> _channels = [];

    public RabbitMqConsumerBackgroundService(
        IEnumerable<RabbitMqConsumerRegistration> registrations,
        IOptions<RabbitMqOptions> options,
        IServiceProvider serviceProvider,
        ILogger<RabbitMqConsumerBackgroundService> logger)
    {
        _registrations = registrations;
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            VirtualHost = _options.VirtualHost,
            UserName = _options.Username,
            Password = _options.Password
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _logger.LogInformation("RabbitMQ consumer connection established to {Host}:{Port}", _options.Host, _options.Port);

        foreach (var registration in _registrations)
        {
            var channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
            _channels.Add(channel);

            await channel.ExchangeDeclareAsync(
                exchange: _options.Exchange,
                type: _options.ExchangeType,
                durable: _options.DurableExchange,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: registration.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await channel.QueueBindAsync(
                queue: registration.QueueName,
                exchange: _options.Exchange,
                routingKey: registration.Topic,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                await HandleMessageAsync(ea, channel, registration, stoppingToken);
            };

            await channel.BasicConsumeAsync(
                queue: registration.QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation(
                "Consuming topic {Topic} from queue {Queue}",
                registration.Topic, registration.QueueName);
        }

        await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
    }

    private async Task HandleMessageAsync(
        BasicDeliverEventArgs ea,
        IChannel channel,
        RabbitMqConsumerRegistration registration,
        CancellationToken cancellationToken)
    {
        try
        {
            var envelopeType = typeof(MessageEnvelope<>).MakeGenericType(registration.MessageType);
            var envelope = JsonSerializer.Deserialize(ea.Body.Span, envelopeType, _options.JsonSerializerOptions);

            using var scope = _serviceProvider.CreateScope();
            var handlerType = typeof(IMessageHandler<>).MakeGenericType(registration.MessageType);
            var handler = scope.ServiceProvider.GetRequiredService(handlerType);

            var handleMethod = handlerType.GetMethod(nameof(IMessageHandler<object>.HandleAsync))!;
            await (Task) handleMethod.Invoke(handler, [envelope, cancellationToken])!;

            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message on topic {Topic}", registration.Topic);
            await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);

        foreach (var channel in _channels)
        {
            await channel.CloseAsync(cancellationToken);
            channel.Dispose();
        }
        _channels.Clear();

        if (_connection is not null)
        {
            await _connection.CloseAsync(cancellationToken);
            _connection.Dispose();
        }
    }
}
