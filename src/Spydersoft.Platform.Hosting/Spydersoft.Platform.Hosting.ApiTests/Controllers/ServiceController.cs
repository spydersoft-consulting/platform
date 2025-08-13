using Microsoft.AspNetCore.Mvc;
using Spydersoft.Platform.Hosting.ApiTests.Services;

namespace Spydersoft.Platform.Hosting.ApiTests.Controllers;

[ApiController]
[Route("[controller]")]
public class ServiceController(IServiceConsumer serviceConsumer1, IServiceConsumer2 serviceConsumer2) : ControllerBase
{

	[HttpGet]
	public IEnumerable<ServiceInfo> Get()
	{
		return new ServiceInfo[] {
			serviceConsumer1.GetInfo(),
			serviceConsumer2.GetInfo()
		};
	}
}