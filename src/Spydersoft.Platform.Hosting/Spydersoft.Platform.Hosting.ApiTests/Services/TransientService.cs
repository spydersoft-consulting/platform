using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.Services;

public interface ITransientService
{
	Guid InstanceId { get; }
	bool IsRunning();
}

[DependencyInjection(typeof(ITransientService), LifetimeOfService.Transient)]
public class TransientService : BaseService, ITransientService
{
}
