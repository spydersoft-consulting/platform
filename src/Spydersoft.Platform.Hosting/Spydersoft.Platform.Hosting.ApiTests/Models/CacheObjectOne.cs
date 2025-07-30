namespace Spydersoft.Platform.Hosting.ApiTests.Models
{
    public class CacheObjectOne
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
