using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Spydersoft.Platform.Hosting.StartupExtensions;
using Spydersoft.Platform.Hosting.Telemetry;
using System.Diagnostics;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Telemetry;

public class ConfigurationFunctionsTests
{
    private bool _traceConfigCalled;
    private bool _metricsConfigCalled;
    private bool _logConfigCalled;

    [SetUp]
    public void Setup()
    {
        _traceConfigCalled = false;
        _metricsConfigCalled = false;
        _logConfigCalled = false;
    }

    [Test]
    public async Task AddSpydersoftTelemetry_WithConfigurationFunctions_ShouldInvokeCallbacks()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Add minimal telemetry configuration
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Telemetry:Enabled"] = "true",
            ["Telemetry:ServiceName"] = "TestService",
            ["Telemetry:ActivitySourceName"] = "TestActivity",
            ["Telemetry:MeterName"] = "TestMeter",
            ["Telemetry:Trace:Type"] = "console",
            ["Telemetry:Metrics:Type"] = "console",
            ["Telemetry:Log:Type"] = "console"
        });

        var configFunctions = new ConfigurationFunctions
        {
            TraceConfiguration = (traceBuilder) =>
            {
                _traceConfigCalled = true;
                Assert.That(traceBuilder, Is.Not.Null);
            },
            MetricsConfiguration = (metricsBuilder) =>
            {
                _metricsConfigCalled = true;
                Assert.That(metricsBuilder, Is.Not.Null);
            },
            LogConfiguration = (logBuilder) =>
            {
                _logConfigCalled = true;
                Assert.That(logBuilder, Is.Not.Null);
            }
        };

        // Act
        builder.AddSpydersoftTelemetry(typeof(ConfigurationFunctionsTests).Assembly, configFunctions);
        var app = builder.Build();

        using (Assert.EnterMultipleScope())
        {
            // Assert - callbacks should have been called during Build()
            // Assert
            Assert.That(_traceConfigCalled, Is.True, "TraceConfiguration should be called");
            Assert.That(_metricsConfigCalled, Is.True, "MetricsConfiguration should be called");
            Assert.That(_logConfigCalled, Is.True, "LogConfiguration should be called");
        }
        await app.DisposeAsync();
    }

    [Test]
    public async Task AddSpydersoftTelemetry_WithNullConfigurationFunctions_ShouldNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Telemetry:Enabled"] = "true",
            ["Telemetry:ServiceName"] = "TestService",
            ["Telemetry:ActivitySourceName"] = "TestActivity",
            ["Telemetry:MeterName"] = "TestMeter",
            ["Telemetry:Trace:Type"] = "console",
            ["Telemetry:Metrics:Type"] = "console",
            ["Telemetry:Log:Type"] = "console"
        });

        // Act & Assert - should not throw
        WebApplication? app = null;
        Assert.DoesNotThrow(() =>
        {
            builder.AddSpydersoftTelemetry(typeof(ConfigurationFunctionsTests).Assembly, null);
            app = builder.Build();
        });

        if (app != null)
        {
            await app.DisposeAsync();
        }
    }

    [Test]
    public async Task AddSpydersoftTelemetry_WithDisabledTelemetry_ShouldNotInvokeCallbacks()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Telemetry:Enabled"] = "false",
            ["Telemetry:ServiceName"] = "TestService"
        });

        var configFunctions = new ConfigurationFunctions
        {
            TraceConfiguration = (traceBuilder) => _traceConfigCalled = true,
            MetricsConfiguration = (metricsBuilder) => _metricsConfigCalled = true,
            LogConfiguration = (logBuilder) => _logConfigCalled = true
        };

        // Act
        builder.AddSpydersoftTelemetry(typeof(ConfigurationFunctionsTests).Assembly, configFunctions);
        var app = builder.Build();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_traceConfigCalled, Is.False, "TraceConfiguration should not be called when telemetry is disabled");
            Assert.That(_metricsConfigCalled, Is.False, "MetricsConfiguration should not be called when telemetry is disabled");
            Assert.That(_logConfigCalled, Is.False, "LogConfiguration should not be called when telemetry is disabled");
        }
        await app.DisposeAsync();
    }

    [Test]
    public void ConfigurationFunctions_Properties_ShouldBeSettable()
    {
        // Arrange & Act
        var configFunctions = new ConfigurationFunctions
        {
            TraceConfiguration = (builder) => { },
            MetricsConfiguration = (builder) => { },
            LogConfiguration = (builder) => { },
            AspNetFilterFunction = (context) => true,
            AspNetRequestEnrichAction = (activity, request) => { },
            AspNetResponseEnrichAction = (activity, response) => { },
            AspNetExceptionEnrichAction = (activity, exception) => { }
        };

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(configFunctions.TraceConfiguration, Is.Not.Null);
            Assert.That(configFunctions.MetricsConfiguration, Is.Not.Null);
            Assert.That(configFunctions.LogConfiguration, Is.Not.Null);
            Assert.That(configFunctions.AspNetFilterFunction, Is.Not.Null);
            Assert.That(configFunctions.AspNetRequestEnrichAction, Is.Not.Null);
            Assert.That(configFunctions.AspNetResponseEnrichAction, Is.Not.Null);
            Assert.That(configFunctions.AspNetExceptionEnrichAction, Is.Not.Null);
        }
    }
}
