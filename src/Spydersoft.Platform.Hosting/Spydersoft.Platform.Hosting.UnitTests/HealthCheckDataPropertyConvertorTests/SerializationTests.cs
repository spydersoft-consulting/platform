using Spydersoft.Platform.Hosting.HealthChecks;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.HealthCheckDataPropertyConvertorTests;

internal class SerializationTests
{
    public JsonSerializerOptions _jsonSerializerOptions = new();

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new HealthCheckDataPropertyConvertor() }
        };
    }

    [TestCaseSource(typeof(SerializationTestData), nameof(SerializationTestData.SerializationTestCases))]
    public string Test_Serialization(HealthCheckResult healthCheckResult)
    {
        string json = JsonSerializer.Serialize(healthCheckResult, _jsonSerializerOptions);
        json = json.Replace("\r\n", "\n");
        Assert.DoesNotThrow(() => JsonDocument.Parse(json), "Invalid JSON");
        return json;
    }

    [TestCaseSource(typeof(SerializationTestData), nameof(SerializationTestData.DeserializaitonTestCases))]
    public void Test_Deserialization(string json, HealthCheckResult expectedHealthCheckResult)
    {
        HealthCheckResult? healthCheckResult = JsonSerializer.Deserialize<HealthCheckResult>(json, _jsonSerializerOptions);

        Assert.Multiple(() =>
        {
            Assert.That(healthCheckResult, Is.Not.Null);
            Assert.That(healthCheckResult!.Status, Is.EqualTo(expectedHealthCheckResult.Status));
            Assert.That(healthCheckResult.Description, Is.EqualTo(expectedHealthCheckResult.Description));
            Assert.That(healthCheckResult.Duration, Is.EqualTo(expectedHealthCheckResult.Duration));
            foreach (var expectedData in expectedHealthCheckResult.ResultData)
            {
                Assert.That(healthCheckResult.ResultData, Contains.Key(expectedData.Key));
                Assert.That(healthCheckResult.ResultData[expectedData.Key].GetType(), Is.EqualTo(expectedData.Value.GetType()));
            }
        });
    }

    [Test]
    public void Test_SerializeDeserialize_RoundTrip_PrimitivesAndComplexTypes()
    {
        var dict = new HealthCheckResult()
        {
            Description = "Test",
            Status = "Healthy",
            Duration = TimeSpan.FromSeconds(1).ToString(),
            ResultData = new Dictionary<string, object>()
                    {
                        { "key1", "value1" },
                        { "key2", "value2" }
                    }
        };
        var json = JsonSerializer.Serialize(dict, _jsonSerializerOptions);
        var deserialized = JsonSerializer.Deserialize<HealthCheckResult>(json, _jsonSerializerOptions);
        Assert.That(deserialized, Is.Not.Null);
        Assert.That(deserialized.Description, Is.EqualTo("Test"));
        Assert.That(deserialized.Status, Is.EqualTo("Healthy"));
        Assert.That(deserialized.Duration, Is.EqualTo(TimeSpan.FromSeconds(1).ToString()));
        Assert.That(deserialized.ResultData, Has.Count.EqualTo(2));
        Assert.That(deserialized.ResultData["key1"], Is.EqualTo("value1"));
        Assert.That(deserialized.ResultData["key2"], Is.EqualTo("value2"));
    }

    private class TestObj
    {
        public int A { get; set; }
        public string B { get; set; } = string.Empty;
    }

    [Test]
    public void Test_SerializeDeserialize_EmptyDictionary()
    {
        var dict = new Dictionary<string, object>();
        var json = JsonSerializer.Serialize(dict, _jsonSerializerOptions);
        var deserialized = JsonSerializer.Deserialize<IReadOnlyDictionary<string, object>>(json, _jsonSerializerOptions);
        Assert.That(deserialized, Is.Not.Null);
        Assert.That(deserialized.Count, Is.EqualTo(0));
    }

    [Test]
    public void Test_Read_ThrowsOnNonObject()
    {
        var json = "[1,2,3]";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<IReadOnlyDictionary<string, object>>(json, _jsonSerializerOptions));
    }

    [TestCase(SerializationTestData.MalformedResultData, "Object not found.")]
    [TestCase(SerializationTestData.MalformedResultData_EmptyProperty, "Invalid key value.")]
    [TestCase(SerializationTestData.MalformedResultData_NoType, "No type information found.")]
    [TestCase(SerializationTestData.MalformedResultData_EmptyType, "No type information found.")]
    [TestCase(SerializationTestData.MalformedResultData_InvalidType, "Type not found.")]
    [TestCase(SerializationTestData.MalformedResultData_EmptyDataKey, "No object data found.")]
    [TestCase(SerializationTestData.MalformedResultData_WrongDataKey, "No object data found.")]
    [TestCase(SerializationTestData.MalformedResultData_NoDataObject, "No object found.")]
    [TestCase(SerializationTestData.MalformedResultData_ObjectTypeMismatch, "Error deserializing object.")]
    public void Fail_InvalidCustomData(string badData, string expectedException)
    {
        JsonException ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<HealthCheckResult>(badData, _jsonSerializerOptions));
        Assert.That(ex.Message, Is.EqualTo(expectedException));
    }
}