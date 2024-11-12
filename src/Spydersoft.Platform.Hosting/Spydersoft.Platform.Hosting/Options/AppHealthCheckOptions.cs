namespace Spydersoft.Platform.Hosting.Options;
public class AppHealthCheckOptions
{
    public const string SectionName = "HealthCheck";

    public bool Enabled { get; set; } = true;

    public string ReadyTags { get; set; } = "ready";

    public string LiveTags { get; set; } = "live";

    public string StartupTags { get; set; } = "startup";

    public List<string> ReadyTagsList()
    {
        return [.. ReadyTags.Split(',', StringSplitOptions.RemoveEmptyEntries)];
    }
    public List<string> LiveTagsList()
    {
        return [.. LiveTags.Split(',', StringSplitOptions.RemoveEmptyEntries)];
    }
    public List<string> StartupTagsList()

    {
        return [.. StartupTags.Split(',', StringSplitOptions.RemoveEmptyEntries)];
    }

}