using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.Services;

public interface IServiceConsumer
{
	ServiceInfo GetInfo();
}

[DependencyInjection(typeof(IServiceConsumer), LifetimeOfService.Scoped)]
public class ServiceConsumer(ISingletonService singleton, IScopedService scoped, ITransientService transient) : IServiceConsumer
{
	public ServiceInfo GetInfo()
	{
		return new ServiceInfo(transient.InstanceId.ToString(), scoped.InstanceId.ToString(), singleton.InstanceId.ToString());
	}

}
