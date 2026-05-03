# Resilience

This library provides the `AddSpydersoftResilience` extension method for `WebApplicationBuilder` that applies a standard resilience pipeline to all HTTP clients registered in the application. The pipeline is built on `Microsoft.Extensions.Http.Resilience` and covers retry, circuit breaker, and timeout policies.

## Getting Started

Add the extension in `Program.cs`:

```csharp
builder.AddSpydersoftResilience();
```

This applies the standard resilience pipeline to every `HttpClient` created via `IHttpClientFactory`.

### Advanced Configuration

Both the top-level options and the standard pipeline can be further tuned programmatically:

```csharp
builder.AddSpydersoftResilience(
    configure: options =>
    {
        // Override options before the pipeline is built
        options.Standard.Retry.MaxRetryAttempts = 5;
    },
    configureStandard: pipeline =>
    {
        // Fine-tune the pipeline directly
        pipeline.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(10);
    }
);
```

## Configuration

Resilience is configured via the `Resilience` section of `appsettings.json`. Programmatic callbacks (see above) are applied after config binding and take precedence.

```json
{
  "Resilience": {
    "Enabled": true,
    "Standard": {
      "TotalRequestTimeout": {
        "Timeout": "00:00:30"
      },
      "Retry": {
        "MaxRetryAttempts": 3,
        "Delay": "00:00:02",
        "BackoffType": "Exponential",
        "UseJitter": true
      },
      "CircuitBreaker": {
        "SamplingDuration": "00:00:30",
        "FailureRatio": 0.1,
        "MinimumThroughput": 100,
        "BreakDuration": "00:00:05"
      },
      "AttemptTimeout": {
        "Timeout": "00:00:10"
      }
    }
  }
}
```

## Configuration Settings

| Setting | Description | Default |
| ------- | ----------- | ------- |
| `Enabled` | Whether the resilience pipeline is applied to HTTP clients. | `true` |
| `Standard` | Standard pipeline options — see sub-sections below. | (see defaults) |

### Standard Pipeline

The pipeline is composed of four ordered strategies:

| Setting | Description | Default |
| ------- | ----------- | ------- |
| `Standard.TotalRequestTimeout.Timeout` | Maximum total time for a request including all retries. | `00:00:30` |
| `Standard.Retry.MaxRetryAttempts` | Number of retry attempts after the initial failure. | `3` |
| `Standard.Retry.Delay` | Base delay between retries. | `00:00:02` |
| `Standard.Retry.BackoffType` | Backoff strategy: `Constant`, `Linear`, or `Exponential`. | `Exponential` |
| `Standard.Retry.UseJitter` | Adds random jitter to retry delays to avoid thundering herd. | `true` |
| `Standard.CircuitBreaker.SamplingDuration` | Window over which failure rate is measured. | `00:00:30` |
| `Standard.CircuitBreaker.FailureRatio` | Failure ratio threshold that trips the circuit (0.0–1.0). | `0.1` |
| `Standard.CircuitBreaker.MinimumThroughput` | Minimum requests in the sampling window before the circuit can trip. | `100` |
| `Standard.CircuitBreaker.BreakDuration` | How long the circuit stays open before allowing a probe request. | `00:00:05` |
| `Standard.AttemptTimeout.Timeout` | Maximum time allowed for a single attempt (before retry). | `00:00:10` |

## Disabling Resilience

Set `Enabled` to `false` to bypass the pipeline entirely — useful for integration test environments where you want raw HTTP behaviour:

```json
{
  "Resilience": {
    "Enabled": false
  }
}
```
