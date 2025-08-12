namespace Spydersoft.Platform.Hosting.ApiTests.Services;

public class BaseService
{
    public Guid InstanceId { get; private set; }

    public bool IsRunning()
    {
        return true;
    }

    public BaseService()
    {
        InstanceId = Guid.NewGuid();
    }
}