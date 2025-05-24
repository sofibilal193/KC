using System.Text.Json;
using System.Text.Json.Serialization;
using KC.Domain.Common.Converters;

namespace KC.Domain.Common.Extensions
{
    public static class JsonExtensions
    {
        public static TValue? GetPropertyValue<TValue>(this JsonElement element, string propertyName)
        {
            foreach (var (property, options) in
                from property in element.EnumerateObject()
                where property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                let options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                select (property, options))
            {
                options.Converters.Add(new JsonStringEnumConverter());
                options.Converters.Add(new SanitizeStringJsonConverter());
                return JsonSerializer.Deserialize<TValue>(property.Value.GetRawText(), options);
            }

            return default;
        }

        public static TValue? GetPropertyValue<TValue>(this JsonDocument document, string propertyName)
        {
            return document.RootElement.GetPropertyValue<TValue>(propertyName);
        }
    }
}
