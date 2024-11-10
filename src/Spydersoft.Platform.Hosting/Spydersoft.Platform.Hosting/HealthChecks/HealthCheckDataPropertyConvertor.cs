using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spydersoft.Platform.Hosting.HealthChecks;
public class HealthCheckDataPropertyConvertor : JsonConverter<IReadOnlyDictionary<string, object>>
{
    public override IReadOnlyDictionary<string, object>? Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
    {
        Dictionary<string, object> dictionary = [];
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            // Get the key.
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string? key = reader.GetString();

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new JsonException("Invalid key value");
            }

            // Get the value.
            reader.Read();
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("No object found");
            }
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName && reader.GetString() != "$type")
            {
                throw new JsonException("No type information found");
            }
            reader.Read();

            string? typeName = reader.GetString() ?? throw new JsonException("No type information found");
            Type? dataType = Type.GetType(typeName, true) ?? throw new JsonException("Type not found");
            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("No object data found");
            }

            reader.Read();

            if (reader.TokenType != JsonTokenType.StartObject && reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("No object found");
            }

            object? dataValue = JsonSerializer.Deserialize(ref reader, dataType, options);
            if (dataValue == null)
            {
                throw new JsonException("Invalid data value");
            }

            // Add to dictionary.
            dictionary.Add(key, dataValue);

            reader.Read();
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyDictionary<string, object> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var item in value)
        {
            writer.WritePropertyName(item.Key);
            writer.WriteStartObject();
            writer.WriteString("$type", item.Value.GetType().AssemblyQualifiedName);
            writer.WritePropertyName("data");
            JsonSerializer.Serialize(writer, item.Value, options);
            writer.WriteEndObject();
        }
        writer.WriteEndObject();
    }
}