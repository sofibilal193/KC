using System.Text.Json;
using System.Text.Json.Serialization;

namespace KC.Domain.Common.Converters
{
    /// <summary>
    /// Converts any source JSON type to string
    /// </summary>
    public class StringJsonConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString()?.Trim() ?? default;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                var stringValue = reader.GetDecimal();
                return stringValue.ToString();
            }
            else if (reader.TokenType == JsonTokenType.False ||
                reader.TokenType == JsonTokenType.True)
            {
                return reader.GetBoolean().ToString();
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                reader.Skip();
                return default;
            }
            else
            {
                return default;
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
