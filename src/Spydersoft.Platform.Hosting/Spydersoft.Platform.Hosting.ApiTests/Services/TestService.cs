namespace Spydersoft.Platform.Hosting.ApiTests.Services;

public interface ITestService
{
    bool IsRunning();
}
public class TestService : ITestService
{
    public bool IsRunning()
    {
        return true;
    }
}
