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

Configuration is controlled by configuration entries in `appsettings.json` or environment variables.  Below are the possible settings with their default values.

```json
"Telemetry": {
  "ActivitySourceName": "Spydersoft.Otel.Activity",
  "AspNetCoreInstrumentation": {
    // AspNetCoreTraceInstrumentationOptions
  },
  "AspNetCoreInstrumentationSection": "AspNetCoreInstrumentation",
  "MeterName": "Spydersoft.Otel.Meter",
  "Otlp": {
    "Endpoint": ""
  },
  "ServiceName": "techradar-data",
  "UseLogExporter": "console",
  "UseMetricsExporter": "console",
  "UseTracingExporter": "console",
  "Zipkin": { 
    // ZipkinExporterOptions  
  },
  "ZipkinConfigurationSection": "Zipkin"
}
```

| Setting                          | Description                                                                          | Possible Values                 |
| -------------------------------- | ------------------------------------------------------------------------------------ | ------------------------------- |
| ActivitySourceName               | The name for the [OpenTelemetry ActivitySource][3]                                   |                                 |
| AspNetCoreInstrumentation        | AspNetCoreTraceInstrumentationOptions                                                | See [AspNetCoreTraceOptions][2] |
| AspNetCoreInstrumentationSection | Section name within `Telemetry` where AspNetCoreTraceInstrumentationOptions are set. |                                 |
| MeterName                        | The name for the OpenTelemetry Meter                                                 |                                 |
| Oltp                             |                                                                                      |                                 |
| ServiceName                      | OpenTelemetry `service.name`                                                         |                                 |
| UseLogExporter                   | Which log exporter to use                                                            | `console`, `oltp`               |
| UseMetricsExporter               | Which metrics exporter to use                                                        | `console`, `oltp`, `prometheus` |
| UseTracingExporter               | Which tracing exporter to use                                                        | `console`, `oltp`, `zipkin`     |
| Zipkin                           | Zipkin Options                                                                       | See [Zipkin Configuration][1]   |
| ZipkinConfigurationSection       | Section name within `Telemetry` where ZipkinConfigurationOptions are set.            |                                 |

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
