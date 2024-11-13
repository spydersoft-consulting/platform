# Spydersoft.Platform.Hosting

Provide extension methods for configuring ASP.NET Core projects with standardized telemetry and authentication.

## Usage

### OpenTelemetry

This library contains an extension method for `WebApplicationBuilder` called `AddSpydersoftTelemetry` that configures OpenTelemetry tracing, metrics, and logging. Add the following line in your `Program.cs`

```csharp
builder.AddSpydersoftTelemetry(typeof(Program).Assembly);
```

The assembly parameter is used to calculate the version and set the OpenTelemetry service version.

#### OpenTelemetry Configuration

Configuration is controlled by configuration entries in `appsettings.json` or environment variables.  Below are the possible settings with their default values.  Notice the `Logging:OpenTelemetry` section.  This section is used to configure the OpenTelemetry SDKs logging providers.

```json
"Logging": {
  "OpenTelemetry": {
    "IncludeFormattedMessage": true,
    "IncludeScopes": true,
    "ParseStateValues": true
  }
},
"Telemetry": {
  "ActivitySourceName": "Spydersoft.Otel.Activity",
  "AspNetCoreInstrumentation": {
    // AspNetCoreTraceInstrumentationOptions
  },
  "Log": {
    "Otlp": {
      "Endpoint": "",
      "Protocol": "grpc"
    },
    "Type": "console"
  },
  "MeterName": "Spydersoft.Otel.Meter",
  "Metrics": {
    "Otlp": {
      "Endpoint": "",
      "Protocol": "grpc"
    },
    "Type": "console"
  },
  "ServiceName": "techradar-data",
  "Trace": {
    "Otlp": {
      "Endpoint": "http://trace.localhost:12345"
    },
    "Type": "console",
    "Zipkin": { 
      // ZipkinExporterOptions  
    },
  },
}
```

| Setting                             | Description                                        | Possible Values                                     |
| ----------------------------------- | -------------------------------------------------- | --------------------------------------------------- |
| Logging:OpenTelemetry               | Configure OpenTelemetry Logging Options            | See [OpenTelemetry Logging Options][5]              |
| Telemetry:ActivitySourceName        | The name for the [OpenTelemetry ActivitySource][3] |                                                     |
| Telemetry:AspNetCoreInstrumentation | AspNetCoreTraceInstrumentationOptions              | See [AspNetCoreTraceOptions][2]                     |
| Telemetry:Log                       | Log Configuration Section                          | See [Log Configuration](#log-configuration)         |
| Telemetry:MeterName                 | The name for the OpenTelemetry Meter               |                                                     |
| Telemetry:Metrics                   | Metrics Configuration Section                      | See [Metrics Configuration](#metrics-configuration) |
| Telemetry:ServiceName               | OpenTelemetry `service.name`                       |                                                     |
| Telemetry:Trace                     | Trace Configuration Section                        | See [Trace Configuration](#trace-configuration)     |

##### Log Configuration

| Setting | Description          | Possible Values                   |
| ------- | -------------------- | --------------------------------- |
| Otlp    | Otlp Options Section | See [Otlp Options](#otlp-options) |
| Type    | Exporter Type        | `console` (default), `otlp`       |

##### Metrics Configuration

| Setting | Description          | Possible Values                           |
| ------- | -------------------- | ----------------------------------------- |
| Otlp    | Otlp Options Section | See [Otlp Options](#otlp-options)         |
| Type    | Exporter Type        | `console` (default), `prometheus`, `otlp` |

##### Trace Configuration

| Setting | Description            | Possible Values                       |
| ------- | ---------------------- | ------------------------------------- |
| Otlp    | Otlp Options Section   | See [Otlp Options](#otlp-options)     |
| Type    | Exporter Type          | `console` (default), `zipkin`, `otlp` |
| Zipkin  | Zipkin Options Section | See [Zipkin Configuration][1]         |

##### Otlp Options

| Setting  | Description                   | Possible Values            |
| -------- | ----------------------------- | -------------------------- |
| Endpoint | Endpoint URL for Otlp logging |                            |
| Protocol | Communication Protocol to use | `grpc` (default) or `http` |

### Identity

This library contains `WebApplicationBuilder` and `IApplicationBuilder` extensions for configuring and using simple JwtBearer authentication and authorization.

A sample implementation in `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

var authInstalled = builder.AddSpydersoftIdentity();

// Other service additions

var app = builder.Build();

app.UseAuthentication(authInstalled)
    .UseRouting()
    .UseAuthorization(authInstalled)
    .UseEndpoints(endpoints => endpoints.MapControllers())
    .UseDefaultFiles()
    .UseStaticFiles();

// Other app Use

app.UseHsts();
await app.RunAsync();

```

Notice that the provided `UseAuthentication` and `UseAuthorization` methods accept a boolean parameter.  If that parameter is true, Authentication/Authorization will be configured using the standard extensions.  These methods must be used to ensure that `UseAuthentication` and `UseAuthorization` are not called if no authentication is configured.

#### Identity Configuration

```json
"Identity": {
  "ApplicationName": "spydersoft-application",
  "Authority": "",
  "Enabled": false
}
```

| Setting         | Description                                                                       |
| --------------- | --------------------------------------------------------------------------------- |
| ApplicationName | The name of the application in the oAuth providers, used to set `ValidAudiences`  |
| Authority       | The URL of the token authority, used to set `ValidIssuers` and bearer `Authority` |
| Enabled         | Whether to enable identity protection or not                                      |

### Serilog

The `AddSpydersoftSerilog` extension on `WebApplicationBuilder` adds a default Serilog console logger plus any Serilog configuration provided in your app settings.  See [Serilog.Settings.Configuration][4] for more details.

> [!IMPORTANT]
> If you add Telemetry above, `AddSpydersoftSerilog` MUST be called with `writeToProviders=true`.  Additionally, Serilog's section of the 
>  appsettings will override the log levels, so log levels must be set in the `"Serilog`" section.

### Health Checks

This library provides extensions to configure (`AddSpydersoftHealthChecks`) and use (`UseSpydersoftHealthChecks`) health checks to provide standard endpoints (`/livez`, `/readyz`, and `/startup`).

#### Configuring New Health Checks

By default, a simple check will return healthy for all endpoints.  To add your own health check:

* Create a new class that implements `IHealthCheck`
* Decorate that class with the `SpydersoftHealthCheckAttribute`, adding tags to control which endpoint(s) will execute the check.
* Call `AddSpydersoftHealthChecks` and `UseSpydersoftHealthChecks` in your `Program.cs`

The [Spydersoft.Platform.Hosting.ApiTests](../Spydersoft.Platform.Hosting.ApiTests/) project has some examples, including some that utilize services in the dependency injection system.

#### Health Check Configuration

Configuration is controlled by configuration entries in `appsettings.json` or environment variables.  Below are the possible settings with their default values.

```json
"HealthChecks": {
  "Enabled": true,
  "LiveTags": "live",  
  "ReadyTags": "ready",
  "StartupTags": "startup"
  
}
```

| Setting     | Description                                                       |
| ----------- | ----------------------------------------------------------------- |
| Enabled     | Whether to enable health checks or not                            |
| LiveTags    | comma separated list of tags to execute for the /livez endpoint   |
| ReadyTags   | comma separated list of tags to execute for the /readyz endpoint  |
| StartupTags | comma separated list of tags to execute for the /startup endpoint |

[1]: https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.Zipkin/README.md "Zipkin Configuration"
[2]: https://github.com/open-telemetry/opentelemetry-dotnet-contrib/tree/main/src/OpenTelemetry.Instrumentation.AspNetCore "AspNetCoreTraceOptions"
[3]: https://opentelemetry.io/docs/languages/net/instrumentation/#setting-up-an-activitysource "OpenTelemetry Activity Source"
[4]: https://github.com/serilog/serilog-settings-configuration "Serilog.Settings.Configuration"
[5]: https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry/Logs/ILogger/OpenTelemetryLoggerOptions.cs "OpenTelemetry Logging Options"
