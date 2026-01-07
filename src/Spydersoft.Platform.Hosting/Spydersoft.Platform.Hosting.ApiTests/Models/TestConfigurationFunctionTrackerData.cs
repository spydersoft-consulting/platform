namespace Spydersoft.Platform.Hosting.ApiTests.Models;

public class TestConfigurationFunctionTrackerData
{
    public int TraceConfigurationCalled { get; set; }
    public int MetricsConfigurationCalled { get; set; }
    public int LogConfigurationCalled { get; set; }
    public int AspNetFilterFunctionCalled { get; set; }
    public int AspNetRequestEnrichActionCalled { get; set; }
    public int AspNetResponseEnrichActionCalled { get; set; }
    public int AspNetExceptionEnrichActionCalled { get; set; }
}