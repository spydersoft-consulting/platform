using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.Services;

public interface IServiceConsumer2
{
	ServiceInfo GetInfo();
}

[DependencyInjection(typeof(IServiceConsumer2), LifetimeOfService.Scoped)]
public class ServiceConsumer2(ISingletonService singleton, IScopedService scoped, ITransientService transient) : IServiceConsumer2
{
	public ServiceInfo GetInfo()
	{
		return new ServiceInfo(transient.InstanceId.ToString(), scoped.InstanceId.ToString(), singleton.InstanceId.ToString());
	}

}
