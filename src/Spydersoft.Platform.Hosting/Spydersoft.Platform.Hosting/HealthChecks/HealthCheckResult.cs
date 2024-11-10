using System.Text.Json.Serialization;

namespace Spydersoft.Platform.Hosting.HealthChecks;

public class HealthCheckResult
{
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;

    [JsonConverter(typeof(HealthCheckDataPropertyConvertor))]
    public IReadOnlyDictionary<string, object> ResultData { get; set; } = new Dictionary<string, object>();
}