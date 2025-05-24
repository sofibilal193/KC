using System.Text.Json;
using System.Text.Json.Serialization;

namespace KC.Domain.Common.Converters
{
    /// <summary>
    /// Trims whitespace from strings when deserializing from JSON. If string
    /// is empty or only contains whitespace, null is returned.
    /// </summary>
    public class TrimStringJsonConverter : JsonConverter<string>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(string);
        }

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && CanConvert(typeToConvert))
            {
                var value = reader.GetString()?.Trim();
                return string.IsNullOrEmpty(value) ? null : value;
            }
            else if (reader.TokenType == JsonTokenType.Number && reader.TryGetDecimal(out decimal value))
            {
                return value.ToString();
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
