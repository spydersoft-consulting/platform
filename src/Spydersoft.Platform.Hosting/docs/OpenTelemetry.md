# OpenTelemetry

This library contains an extension method for `WebApplicationBuilder` called `AddSpydersoftTelemetry` that configures OpenTelemetry tracing, metrics, and logging. Add the following line in your `Program.cs`

```csharp
builder.AddSpydersoftTelemetry(typeof(Program).Assembly);
```

The assembly parameter is used to calculate the version and set the OpenTelemetry service version.

### Advanced Configuration

For advanced scenarios, you can provide a `ConfigurationFunctions` object to customize OpenTelemetry behavior:

```csharp
using Spydersoft.Platform.Hosting.Telemetry;

var configFunctions = new ConfigurationFunctions
{
    // Add custom trace configuration
    TraceConfiguration = (builder) => {
        // Add custom sources, processors, etc.
    },
    
    // Add custom metrics configuration
    MetricsConfiguration = (builder) => {
        // Add custom meters, views, etc.
    },
    
    // Add custom logging configuration
    LogConfiguration = (builder) => {
        // Add custom log processors, etc.
    },
    
    // Filter ASP.NET Core requests
    AspNetFilterFunction = (httpContext) => {
        // Return false to exclude requests from tracing
        return !httpContext.Request.Path.StartsWithSegments("/health");
    },
    
    // Enrich traces with request data
    AspNetRequestEnrichAction = (activity, request) => {
        activity.SetTag("custom.request.tag", "value");
    },
    
    // Enrich traces with response data
    AspNetResponseEnrichAction = (activity, response) => {
        activity.SetTag("custom.response.tag", "value");
    },
    
    // Enrich traces with exception data
    AspNetExceptionEnrichAction = (activity, exception) => {
        activity.SetTag("custom.exception.tag", exception.Message);
    }
};

builder.AddSpydersoftTelemetry(typeof(Program).Assembly, configFunctions);
```

## OpenTelemetry Configuration

Configuration is controlled by configuration entries in `appsettings.json` or environment variables. Below are the possible settings with their default values. Notice the `Logging:OpenTelemetry` section. This section is used to configure the OpenTelemetry SDKs logging providers.

### Environment Variable Support

The following environment variables are supported and will override corresponding appsettings.json values when set:

- `OTEL_EXPORTER_OTLP_ENDPOINT` - Overrides the OTLP endpoint for all telemetry types (logs, metrics, traces)
- `OTEL_EXPORTER_OTLP_PROTOCOL` - Overrides the OTLP protocol (`grpc`, `http`, or `http/protobuf`)
- `OTEL_EXPORTER_OTLP_HEADERS` - Overrides the OTLP headers for authentication and metadata (format: `key1=value1,key2=value2`)

These environment variables take precedence over the configuration file settings, allowing for easier deployment and container configuration. This is particularly useful for managing sensitive authentication tokens in containerized environments.

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
  "Enabled": true,
  "Log": {
    "Otlp": {
      "Endpoint": "",
      "Headers": {
        "Authorization": "Bearer token"
      },
      "Protocol": "grpc"
    },
    "Type": "console"
  },
  "MeterName": "Spydersoft.Otel.Meter",
  "Metrics": {
    "HistogramAggregation": "exponential",
    "Otlp": {
      "Endpoint": "",
      "Headers": {
        "Authorization": "Bearer token"
      },
      "Protocol": "grpc"
    },
    "Type": "console"
  },
  "ServiceName": "techradar-data",
  "Trace": {
    "Otlp": {
      "Endpoint": "http://trace.localhost:12345",
      "Headers": {
        "Authorization": "Bearer token"
      }
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
| Logging:OpenTelemetry               | Configure OpenTelemetry Logging Options            | See [OpenTelemetry Logging Options][4]              |
| Telemetry:ActivitySourceName        | The name for the [OpenTelemetry ActivitySource][3] |                                                     |
| Telemetry:AspNetCoreInstrumentation | AspNetCoreTraceInstrumentationOptions              | See [AspNetCoreTraceOptions][2]                     |
| Telemetry:Enabled                   | Enable or disable OpenTelemetry                    | `true` (default), `false`                           |
| Telemetry:Log                       | Log Configuration Section                          | See [Log Configuration](#log-configuration)         |
| Telemetry:MeterName                 | The name for the OpenTelemetry Meter               |                                                     |
| Telemetry:Metrics                   | Metrics Configuration Section                      | See [Metrics Configuration](#metrics-configuration) |
| Telemetry:ServiceName               | OpenTelemetry `service.name`                       |                                                     |
| Telemetry:Trace                     | Trace Configuration Section                        | See [Trace Configuration](#trace-configuration)     |

### Log Configuration

| Setting | Description          | Possible Values                           |
| ------- | -------------------- | ----------------------------------------- |
| Otlp    | Otlp Options Section | See [Otlp Options](#otlp-options)         |
| Type    | Exporter Type        | `console` (default), `otlp`, `none`       |

### Metrics Configuration

| Setting              | Description                         | Possible Values                                    |
| -------------------- | ----------------------------------- | -------------------------------------------------- |
| HistogramAggregation | Histogram aggregation strategy      | empty (default explicit bounds), `exponential`     |
| Otlp                 | Otlp Options Section                | See [Otlp Options](#otlp-options)                  |
| Type                 | Exporter Type                       | `console` (default), `prometheus`, `otlp`, `none`  |

### Trace Configuration

| Setting | Description            | Possible Values                               |
| ------- | ---------------------- | --------------------------------------------- |
| Otlp    | Otlp Options Section   | See [Otlp Options](#otlp-options)             |
| Type    | Exporter Type          | `console` (default), `zipkin`, `otlp`, `none` |
| Zipkin  | Zipkin Options Section | See [Zipkin Configuration][1]                 |

#### Otlp Options

| Setting  | Description                                        | Possible Values                              |
| -------- | -------------------------------------------------- | -------------------------------------------- |
| Endpoint | Endpoint URL for Otlp logging                      |                              |
| Headers  | Dictionary of headers for authentication/metadata  | Key-value pairs (overridden by `OTEL_EXPORTER_OTLP_HEADERS` env var) |
| Protocol | Communication Protocol to use                      | `grpc` (default), `http`, or `http/protobuf` |

[1]: https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.Zipkin/README.md "Zipkin Configuration"
[2]: https://github.com/open-telemetry/opentelemetry-dotnet-contrib/tree/main/src/OpenTelemetry.Instrumentation.AspNetCore "AspNetCoreTraceOptions"
[3]: https://opentelemetry.io/docs/languages/net/instrumentation/#setting-up-an-activitysource "OpenTelemetry Activity Source"
[4]: https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry/Logs/ILogger/OpenTelemetryLoggerOptions.cs "OpenTelemetry Logging Options"
