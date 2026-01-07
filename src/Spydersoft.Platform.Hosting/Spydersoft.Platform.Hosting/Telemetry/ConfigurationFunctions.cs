using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Spydersoft.Platform.Hosting.Telemetry;

/// <summary>
/// Provides configuration functions for customizing OpenTelemetry behavior, including
/// tracing, metrics, logging, and ASP.NET Core instrumentation enrichment.
/// </summary>
public class ConfigurationFunctions
{
    /// <summary>
    /// Gets or sets an action to configure additional tracing options.
    /// Use this to add custom sources, processors, or other trace-specific configuration.
    /// </summary>
    public Action<TracerProviderBuilder>? TraceConfiguration { get; set; }

    /// <summary>
    /// Gets or sets an action to configure additional metrics options.
    /// Use this to add custom meters, views, or other metrics-specific configuration.
    /// </summary>
    public Action<MeterProviderBuilder>? MetricsConfiguration { get; set; }

    /// <summary>
    /// Gets or sets an action to configure additional logging options.
    /// Use this to add custom log processors or other logging-specific configuration.
    /// </summary>
    public Action<LoggerProviderBuilder>? LogConfiguration { get; set; }

    /// <summary>
    /// Gets or sets a filter function to determine which ASP.NET Core requests should be traced.
    /// Return <c>false</c> to exclude a request from tracing (e.g., health check endpoints).
    /// </summary>
    /// <remarks>
    /// This function is called for each incoming HTTP request before tracing begins.
    /// </remarks>
    public Func<HttpContext, bool>? AspNetFilterFunction { get; set; }

    /// <summary>
    /// Gets or sets an action to enrich traces with custom data from the HTTP request.
    /// Use this to add custom tags or attributes based on the incoming request.
    /// </summary>
    /// <remarks>
    /// This action is called after the request is received but before the response is sent.
    /// </remarks>
    public Action<Activity, HttpRequest>? AspNetRequestEnrichAction { get; set; }

    /// <summary>
    /// Gets or sets an action to enrich traces with custom data from the HTTP response.
    /// Use this to add custom tags or attributes based on the outgoing response.
    /// </summary>
    /// <remarks>
    /// This action is called after the response is generated.
    /// </remarks>
    public Action<Activity, HttpResponse>? AspNetResponseEnrichAction { get; set; }

    /// <summary>
    /// Gets or sets an action to enrich traces with custom data when an exception occurs.
    /// Use this to add custom tags or attributes based on exception details.
    /// </summary>
    /// <remarks>
    /// This action is called when an unhandled exception occurs during request processing.
    /// </remarks>
    public Action<Activity, Exception>? AspNetExceptionEnrichAction { get; set; }
}