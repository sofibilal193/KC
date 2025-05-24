using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KC.Persistence.Common.Converters
{
    public class JsonValueConverter<T> : ValueConverter<T?, string?> where T : class
    {
        public JsonValueConverter() : base
        (
            value => value == null ? null : JsonSerializer.Serialize(value, JsonSerializerOptions.Default),
            json => json == null ? null : JsonSerializer.Deserialize<T>(json, JsonSerializerOptions.Default)
        )
        { }
    }
}
