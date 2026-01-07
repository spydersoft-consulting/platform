using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Spydersoft.Platform.Hosting.ApiTests.Models;
using Spydersoft.Platform.Hosting.ApiTests.OptionsTests;
using Spydersoft.Platform.Hosting.ApiTests.Services;

namespace Spydersoft.Platform.Hosting.ApiTests.Controllers;

[ApiController]
[Route("[controller]")]
public class TelemetryFunctionsAccessController() : ControllerBase
{
    [HttpGet()]
    public TestConfigurationFunctionTrackerData GetTracker()
    {
        return TestConfigurationFunctionTracker.Instance.Data;
    }
}
