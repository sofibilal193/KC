using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KC.Domain.Common.Converters
{
    /// <summary>
    /// Converts to and from a JSON date value in yyyy-MM-dd format
    /// If string is empty or only contains whitespace, null is returned.
    /// </summary>
    public class RoundTripDateJsonConverter : JsonConverter<DateTime?>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(DateTime?);
        }

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString()?.Trim();
            if (!string.IsNullOrEmpty(value) && DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                return dt;
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }
    }
}
