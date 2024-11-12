namespace Spydersoft.Platform.Hosting.UnitTests.HealthCheckDataPropertyConvertorTests;
public class ComplexObject
{
    public string Name { get; set; } = string.Empty;

    public int Age { get; set; } = 0;

    public SimpleObject SimpleObject { get; set; } = new SimpleObject();

    public Dictionary<string, SimpleObject> SimpleObjectDictionary { get; set; } = [];
}