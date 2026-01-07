using Spydersoft.Platform.Hosting.ApiTests.Models;

namespace Spydersoft.Platform.Hosting.ApiTests.Services;

public class TestConfigurationFunctionTracker
{
    private static TestConfigurationFunctionTracker? _instance;
    public static TestConfigurationFunctionTracker Instance
    {
        get
        {
            _instance ??= new TestConfigurationFunctionTracker();
            return _instance;
        }
    }

    public TestConfigurationFunctionTrackerData Data { get; } = new TestConfigurationFunctionTrackerData();

    public void Reset()
    {
        Data.TraceConfigurationCalled = 0;
        Data.MetricsConfigurationCalled = 0;
        Data.LogConfigurationCalled = 0;
        Data.AspNetFilterFunctionCalled = 0;
        Data.AspNetRequestEnrichActionCalled = 0;
        Data.AspNetResponseEnrichActionCalled = 0;
        Data.AspNetExceptionEnrichActionCalled = 0;
    }
}

