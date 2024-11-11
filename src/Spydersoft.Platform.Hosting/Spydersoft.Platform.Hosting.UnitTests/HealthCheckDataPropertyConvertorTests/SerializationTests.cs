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