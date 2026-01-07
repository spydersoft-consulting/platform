using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Spydersoft.Platform.Hosting.Telemetry;

public class ConfigurationFunctions
{
    public Action<TracerProviderBuilder>? TraceConfiguration { get; set; }
    public Action<MeterProviderBuilder>? MetricsConfiguration { get; set; }
    public Action<LoggerProviderBuilder>? LogConfiguration { get; set; }

    public Func<HttpContext, bool>? AspNetFilterFunction { get; set; }
    public Action<Activity, HttpRequest>? AspNetRequestEnrichAction { get; set; }
    public Action<Activity, HttpResponse>? AspNetResponseEnrichAction { get; set; }
    public Action<Activity, Exception>? AspNetExceptionEnrichAction { get; set; }
}