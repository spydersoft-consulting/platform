using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Spydersoft.Messaging.RabbitMQ.Options;

namespace Spydersoft.Messaging.RabbitMQ.IntegrationTests;

internal static class TestHelpers
{
    public static RabbitMqOptions BrokerOptions(string exchange, Action<RabbitMqOptions>? customize = null)
    {
        var options = new RabbitMqOptions
        {
            Host = RabbitMqContainerFixture.Host,
            Port = RabbitMqContainerFixture.Port,
            Username = RabbitMqContainerFixture.Username,
            Password = RabbitMqContainerFixture.Password,
            Exchange = exchange,
            ExchangeType = "topic",
            DurableExchange = false,
            PersistentMessages = false,
        };
        customize?.Invoke(options);
        return options;
    }

    public static RabbitMqMessagePublisher BuildPublisher(RabbitMqOptions options) =>
        new(Microsoft.Extensions.Options.Options.Create(options),
            LoggerFactory.Create(b => { }).CreateLogger<RabbitMqMessagePublisher>());

    public static IHost BuildConsumerHost(
        RabbitMqOptions options,
        Action<ISpydersoftMessagingBuilder> registerConsumers)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Logging.ClearProviders();

        var messagingBuilder = builder.Services.AddSpydersoftRabbitMq(opts =>
        {
            opts.Host = options.Host;
            opts.Port = options.Port;
            opts.VirtualHost = options.VirtualHost;
            opts.Username = options.Username;
            opts.Password = options.Password;
            opts.Exchange = options.Exchange;
            opts.ExchangeType = options.ExchangeType;
            opts.DurableExchange = options.DurableExchange;
            opts.PersistentMessages = options.PersistentMessages;
            opts.JsonSerializerOptions = options.JsonSerializerOptions;
        });

        registerConsumers(messagingBuilder);

        return builder.Build();
    }

    public static async Task<bool> WaitForAsync(Func<bool> condition, TimeSpan timeout)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.Elapsed < timeout)
        {
            if (condition())
            {
                return true;
            }
            await Task.Delay(50);
        }
        return condition();
    }

    /// <summary>
    /// Waits for the named queue to exist on the broker. The consumer background service
    /// declares queues asynchronously inside ExecuteAsync, which races with the test's
    /// publish call — polling QueueDeclarePassive ensures the binding is in place before we publish.
    /// </summary>
    public static async Task WaitForQueueAsync(RabbitMqOptions options, string queueName, TimeSpan timeout)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.Host,
            Port = options.Port,
            UserName = options.Username,
            Password = options.Password,
        };
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.Elapsed < timeout)
        {
            try
            {
                await using var conn = await factory.CreateConnectionAsync();
                await using var channel = await conn.CreateChannelAsync();
                await channel.QueueDeclarePassiveAsync(queueName);
                return;
            }
            catch
            {
                await Task.Delay(100);
            }
        }
        throw new TimeoutException($"Queue '{queueName}' was not declared within {timeout}.");
    }
}
