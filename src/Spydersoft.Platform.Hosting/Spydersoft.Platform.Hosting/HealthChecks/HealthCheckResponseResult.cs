namespace Spydersoft.Platform.Hosting.HealthChecks;
public class HealthCheckResponseResult
{
    public string Status { get; set; } = string.Empty;
    public string TotalDuration { get; set; } = string.Empty;
    public Dictionary<string, HealthCheckResult>? Results { get; set; }
}