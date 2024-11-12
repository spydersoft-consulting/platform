using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.HealthChecks;
static class HealthCheckWriter
{
    public static Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var results = new Dictionary<string, HealthCheckResult>();
        foreach (var healthReportEntry in healthReport.Entries)
        {
            var healthCheckResult = new HealthCheckResult
            {
                Status = healthReportEntry.Value.Status.ToString(),
                Description = healthReportEntry.Value.Description ?? string.Empty,
                Duration = healthReportEntry.Value.Duration.ToString(),
                ResultData = healthReportEntry.Value.Data
            };

            results.Add(healthReportEntry.Key, healthCheckResult);
        }

        var responseResult = new HealthCheckResponseResult
        {
            Status = healthReport.Status.ToString(),
            TotalDuration = healthReport.TotalDuration.ToString(),
            Results = results
        };

        return context.Response.WriteAsJsonAsync(responseResult, serializerOptions);
    }
}