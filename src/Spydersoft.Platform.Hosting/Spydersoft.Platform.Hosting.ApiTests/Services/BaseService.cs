namespace Spydersoft.Platform.Hosting.ApiTests.Services;

public class BaseService
{
    public Guid InstanceId { get; private set; }

    public BaseService()
    {
        InstanceId = Guid.NewGuid();
    }
}