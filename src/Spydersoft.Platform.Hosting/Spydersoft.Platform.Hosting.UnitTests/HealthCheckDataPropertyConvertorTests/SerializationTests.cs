using Spydersoft.Platform.Hosting.HealthChecks;
using Spydersoft.Platform.Hosting.Options;
using Spydersoft.Platform.Hosting.UnitTests.ApiTests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            WriteIndented = true
        };
    }

    [TestCaseSource(typeof(SerializationTestData), nameof(SerializationTestData.SerializationTestCases))]
    public string Test_Serialization(HealthCheckResult healthCheckResult)
    {
        string json = JsonSerializer.Serialize(healthCheckResult, _jsonSerializerOptions);

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
            //Assert.That(healthCheckResult.ResultData, Is.EquivalentTo(expectedHealthCheckResult.ResultData));
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