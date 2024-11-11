using Spydersoft.Platform.Hosting.HealthChecks;
using System.Collections;
using System.Net.NetworkInformation;

namespace Spydersoft.Platform.Hosting.UnitTests.HealthCheckDataPropertyConvertorTests;
public static class SerializationTestData
{
    #region Test1 Data
    public static HealthCheckResult Test1 = new()
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

    public const string Test1Json = "{\n  \"description\": \"Test\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": {\n    \"key1\": {\n      \"$type\": \"System.String, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\",\n      \"data\": \"value1\"\n    },\n    \"key2\": {\n      \"$type\": \"System.String, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\",\n      \"data\": \"value2\"\n    }\n  }\n}";
    #endregion

    #region Test2 Data
    public static HealthCheckResult Test2 = new()
    {
        Description = "Test2",
        Status = "Healthy",
        Duration = TimeSpan.FromSeconds(1).ToString(),
        ResultData = new Dictionary<string, object>()
                    {
                        { "key1", new List<SimpleObject>() {
                            new() {
                                 Name = "Name1",
                                 DateCreated = new DateTime(2000,1,1,0,0,0, DateTimeKind.Utc),
                                 Age = 1
                            },
                            new() {
                                 Name = "Name2",
                                 DateCreated = new DateTime(2002,1,10,0,0,0, DateTimeKind.Utc),
                                 Age = 2
                            },
                            }
                        },
                        { "key2", new SimpleObject() {
                                 Name = "Name3",
                                 DateCreated = new DateTime(2009,10,1,0,0,0, DateTimeKind.Utc),
                                 Age = 3
                            }
                        }
                    }
    };

    public const string Test2Json = "{\n  \"description\": \"Test2\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": {\n    \"key1\": {\n      \"$type\": \"System.Collections.Generic.List\\u00601[[Spydersoft.Platform.Hosting.UnitTests.HealthCheckDataPropertyConvertorTests.SimpleObject, Spydersoft.Platform.Hosting.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\",\n      \"data\": [\n        {\n          \"name\": \"Name1\",\n          \"dateCreated\": \"2000-01-01T00:00:00Z\",\n          \"age\": 1\n        },\n        {\n          \"name\": \"Name2\",\n          \"dateCreated\": \"2002-01-10T00:00:00Z\",\n          \"age\": 2\n        }\n      ]\n    },\n    \"key2\": {\n      \"$type\": \"Spydersoft.Platform.Hosting.UnitTests.HealthCheckDataPropertyConvertorTests.SimpleObject, Spydersoft.Platform.Hosting.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\n      \"data\": {\n        \"name\": \"Name3\",\n        \"dateCreated\": \"2009-10-01T00:00:00Z\",\n        \"age\": 3\n      }\n    }\n  }\n}";
    #endregion

    #region Test3 Data
    public static HealthCheckResult Test3 = new()
    {
        Description = "Test3",
        Status = "Healthy",
        Duration = TimeSpan.FromSeconds(1).ToString(),
        ResultData = new Dictionary<string, object>()
                    {
                        { "key1", new List<ComplexObject>() {
                            new()
                            {
                                 Name = "Complex Object 1",
                                 Age = 1,
                                 SimpleObjectDictionary = new Dictionary<string, SimpleObject>()
                                 {
                                     { "key1", new SimpleObject() {
                                         Name = "Name1",
                                         DateCreated = new DateTime(2000,1,1,0,0,0, DateTimeKind.Utc),
                                         Age = 1
                                     }},
                                     { "key2", new SimpleObject() {
                                         Name = "Name2",
                                         DateCreated = new DateTime(2002,1,10,0,0,0, DateTimeKind.Utc),
                                         Age = 2
                                     }}
                                 },
                                 SimpleObject = new SimpleObject()
                                 {
                                     Name = "Name1",
                                     DateCreated = new DateTime(2000,1,1,0,0,0, DateTimeKind.Utc),
                                     Age = 1
                                 }
                            }
                        }
                    }
        }
    };

    public const string Test3Data = "{\n  \"description\": \"Test3\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": {\n    \"key1\": {\n      \"$type\": \"System.Collections.Generic.List\\u00601[[Spydersoft.Platform.Hosting.UnitTests.HealthCheckDataPropertyConvertorTests.ComplexObject, Spydersoft.Platform.Hosting.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\",\n      \"data\": [\n        {\n          \"name\": \"Complex Object 1\",\n          \"age\": 1,\n          \"simpleObject\": {\n            \"name\": \"Name1\",\n            \"dateCreated\": \"2000-01-01T00:00:00Z\",\n            \"age\": 1\n          },\n          \"simpleObjectDictionary\": {\n            \"key1\": {\n              \"name\": \"Name1\",\n              \"dateCreated\": \"2000-01-01T00:00:00Z\",\n              \"age\": 1\n            },\n            \"key2\": {\n              \"name\": \"Name2\",\n              \"dateCreated\": \"2002-01-10T00:00:00Z\",\n              \"age\": 2\n            }\n          }\n        }\n      ]\n    }\n  }\n}";
    #endregion

    #region Bad JSON Data

    public const string MalformedResultData = "{\n  \"description\": \"Test\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": [ \"\" ] }";

    public const string MalformedResultData_EmptyProperty = "{\n  \"description\": \"Test\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": { \"\": { \"$type\": \"<>f__AnonymousType0, Spydersoft.Platform.Hosting.ApiTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\", \"data\": {} } , {} } }";

    public const string MalformedResultData_NoType = "{\n  \"description\": \"Test\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": { \"first\": { \"data\": {} } } }";

    public const string MalformedResultData_EmptyType = "{\n  \"description\": \"Test\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": { \"first\": { \"$type\": \"\", \"data\": {} } , {} } }";

    public const string MalformedResultData_InvalidType = "{\n  \"description\": \"Test\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": { \"first\": { \"$type\": \"InvalidType\", \"data\": {} } , {} } }";

    public const string MalformedResultData_EmptyDataKey = "{\n  \"description\": \"Test\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": { \"first\": { \"$type\": \"<>f__AnonymousType0, Spydersoft.Platform.Hosting.ApiTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\", \"\": [] } } }";

    public const string MalformedResultData_WrongDataKey = "{\n  \"description\": \"Test\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": { \"first\": { \"$type\": \"<>f__AnonymousType0, Spydersoft.Platform.Hosting.ApiTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\", \"data2\": {} } } }";

    public const string MalformedResultData_NoDataObject = "{\n  \"description\": \"Test\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": { \"first\": { \"$type\": \"<>f__AnonymousType0, Spydersoft.Platform.Hosting.ApiTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\", \"data\": \"\" } } }";

    public const string MalformedResultData_ObjectTypeMismatch = "{\n  \"description\": \"Test\",\n  \"status\": \"Healthy\",\n  \"duration\": \"00:00:01\",\n  \"resultData\": { \"first\": { \"$type\": \"System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e],[System.String, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\", \"data\": [\"string1\",\"string2\"] } } }";



    #endregion

    public static IEnumerable SerializationTestCases
    {
        get
        {
            yield return new TestCaseData(Test1).Returns(Test1Json);

            yield return new TestCaseData(Test2).Returns(Test2Json);

            yield return new TestCaseData(Test3).Returns(Test3Data);
        }
    }

    public static IEnumerable DeserializaitonTestCases
    {
        get
        {
            yield return new TestCaseData(Test1Json, Test1);

            yield return new TestCaseData(Test2Json, Test2);

            yield return new TestCaseData(Test3Data, Test3);
        }
    }
}