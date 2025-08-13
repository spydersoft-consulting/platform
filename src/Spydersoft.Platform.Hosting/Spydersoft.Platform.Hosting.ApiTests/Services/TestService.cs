namespace Spydersoft.Platform.Hosting.ApiTests.Services;

public interface ITestService
{
    bool IsRunning();
}
public class TestService : ITestService
{
    private readonly System.Timers.Timer _timer;
    private bool _isRunning;
    private readonly object _runningObject = new object();

    public TestService()
    {
        _isRunning = false;
        _timer = new System.Timers.Timer(5000);
        _timer.Elapsed += (s, e) =>
        {
            lock (_runningObject)
            {
                _isRunning = true;
            }
            _timer.Stop();
        };
        _timer.Start();
    }

    public bool IsRunning()
    {
        return _isRunning;
    }
}
