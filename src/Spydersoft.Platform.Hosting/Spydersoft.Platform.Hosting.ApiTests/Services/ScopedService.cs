using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.Services;

public interface IScopedService
{
	Guid InstanceId { get; }
	bool IsRunning();
}

[DependencyInjection(typeof(IScopedService), LifetimeOfService.Scoped)]
public class ScopedService : BaseService, IScopedService
{
}