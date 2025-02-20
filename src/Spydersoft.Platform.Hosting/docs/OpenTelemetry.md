# OpenTelemetry

This library contains an extension method for `WebApplicationBuilder` called `AddSpydersoftTelemetry` that configures OpenTelemetry tracing, metrics, and logging. Add the following line in your `Program.cs`

```csharp
builder.AddSpydersoftTelemetry(typeof(Program).Assembly);
```

The assembly parameter is used to calculate the version and set the OpenTelemetry service version.

## OpenTelemetry Configuration

Configuration is controlled by configuration entries in `appsettings.json` or environment variables. Below are the possible settings with their default values. Notice the `Logging:OpenTelemetry` section. This section is used to configure the OpenTelemetry SDKs logging providers.

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
| Logging:OpenTelemetry               | Configure OpenTelemetry Logging Options            | See [OpenTelemetry Logging Options][4]              |
| Telemetry:ActivitySourceName        | The name for the [OpenTelemetry ActivitySource][3] |                                                     |
| Telemetry:AspNetCoreInstrumentation | AspNetCoreTraceInstrumentationOptions              | See [AspNetCoreTraceOptions][2]                     |
| Telemetry:Log                       | Log Configuration Section                          | See [Log Configuration](#log-configuration)         |
| Telemetry:MeterName                 | The name for the OpenTelemetry Meter               |                                                     |
| Telemetry:Metrics                   | Metrics Configuration Section                      | See [Metrics Configuration](#metrics-configuration) |
| Telemetry:ServiceName               | OpenTelemetry `service.name`                       |                                                     |
| Telemetry:Trace                     | Trace Configuration Section                        | See [Trace Configuration](#trace-configuration)     |

### Log Configuration

| Setting | Description          | Possible Values                   |
| ------- | -------------------- | --------------------------------- |
| Otlp    | Otlp Options Section | See [Otlp Options](#otlp-options) |
| Type    | Exporter Type        | `console` (default), `otlp`       |

### Metrics Configuration

| Setting | Description          | Possible Values                           |
| ------- | -------------------- | ----------------------------------------- |
| Otlp    | Otlp Options Section | See [Otlp Options](#otlp-options)         |
| Type    | Exporter Type        | `console` (default), `prometheus`, `otlp` |

### Trace Configuration

| Setting | Description            | Possible Values                       |
| ------- | ---------------------- | ------------------------------------- |
| Otlp    | Otlp Options Section   | See [Otlp Options](#otlp-options)     |
| Type    | Exporter Type          | `console` (default), `zipkin`, `otlp` |
| Zipkin  | Zipkin Options Section | See [Zipkin Configuration][1]         |

#### Otlp Options

| Setting  | Description                   | Possible Values            |
| -------- | ----------------------------- | -------------------------- |
| Endpoint | Endpoint URL for Otlp logging |                            |
| Protocol | Communication Protocol to use | `grpc` (default) or `http` |

[1]: https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.Zipkin/README.md "Zipkin Configuration"
[2]: https://github.com/open-telemetry/opentelemetry-dotnet-contrib/tree/main/src/OpenTelemetry.Instrumentation.AspNetCore "AspNetCoreTraceOptions"
[3]: https://opentelemetry.io/docs/languages/net/instrumentation/#setting-up-an-activitysource "OpenTelemetry Activity Source"
[4]: https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry/Logs/ILogger/OpenTelemetryLoggerOptions.cs "OpenTelemetry Logging Options"
