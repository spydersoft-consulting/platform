using Testcontainers.RabbitMq;

namespace Spydersoft.Messaging.RabbitMQ.IntegrationTests;

[SetUpFixture]
public sealed class RabbitMqContainerFixture
{
    private const string TestUsername = "guest";
    private const string TestPassword = "guest";

    public static RabbitMqContainer Container { get; private set; } = null!;

    public static string Host => Container.Hostname;
    public static int Port => Container.GetMappedPublicPort(5672);
    public static string Username => TestUsername;
    public static string Password => TestPassword;

    [OneTimeSetUp]
    public async Task GlobalSetUp()
    {
        Container = new RabbitMqBuilder("rabbitmq:3.13-management")
            .WithUsername(TestUsername)
            .WithPassword(TestPassword)
            .Build();

        await Container.StartAsync();
    }

    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        if (Container is not null)
        {
            await Container.DisposeAsync();
        }
    }
}
