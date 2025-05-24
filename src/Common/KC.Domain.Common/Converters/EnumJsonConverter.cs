using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using KC.Domain.Common.Exceptions;

namespace KC.Domain.Common.Converters
{
    public class EnumJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(typeof(CustomConverter<>).MakeGenericType(typeToConvert))!;
        }

        internal sealed class CustomConverter<T> : JsonConverter<T> where T : struct, Enum
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var valueToConvert = reader.GetString();
                foreach (var value in Enum.GetValues<T>())
                {
                    var attribute = typeof(T).GetField(value.ToString())?.GetCustomAttribute<JsonEnumValueAttribute>();
                    if (attribute?.Value == valueToConvert)
                    {
                        return value;
                    }
                }
                if (Enum.TryParse(valueToConvert, out T parsedValue) && Enum.IsDefined(parsedValue))
                {
                    return parsedValue;
                }
                throw new DomainException($"The enum '{typeof(T)}' does not contain the value: '{valueToConvert}'.");
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                var attribute = typeof(T).GetField(value.ToString())?.GetCustomAttribute<JsonEnumValueAttribute>();
                writer.WriteStringValue(attribute?.Value ?? value.ToString());
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonEnumValueAttribute : Attribute
    {
        public string Value { get; }

        public JsonEnumValueAttribute(string value)
        {
            Value = value;
        }
    }
}
