using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.Services;

public interface ITransientService
{
	Guid InstanceId { get; }
}

[DependencyInjection(typeof(ITransientService), LifetimeOfService.Transient)]
public class TransientService : BaseService, ITransientService
{
}
