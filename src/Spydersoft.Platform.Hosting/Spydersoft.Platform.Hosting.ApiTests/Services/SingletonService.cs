using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.Services;

public interface ISingletonService
{
	Guid InstanceId { get; }
}

[DependencyInjection(typeof(ISingletonService), LifetimeOfService.Singleton)]
public class SingletonService : BaseService, ISingletonService
{
}