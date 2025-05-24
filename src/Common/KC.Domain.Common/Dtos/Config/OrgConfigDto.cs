using System.Text.Json;
using KC.Domain.Common.Config;
using KC.Domain.Common.Extensions;
using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public record OrgConfigDto
    {
        /// <summary>
        /// The unique identifier for the org config.
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; init; }

        /// <summary>
        /// Category of org Config.
        /// </summary>
        [ProtoMember(2)]
        public string Category { get; init; } = "";

        /// <summary>
        /// Type of field of org Config.
        /// </summary>
        [ProtoMember(3)]
        public FieldType FieldType { get; init; }

        /// <summary>
        /// Name of org Config.
        /// </summary>
        [ProtoMember(4)]
        public string Name { get; init; } = "";

        /// <summary>
        /// Description of org Config.
        /// </summary>
        [ProtoMember(5)]
        public string? Description { get; init; }

        /// <summary>
        /// Display order of org Config.
        /// </summary>
        [ProtoMember(6)]
        public short DisplayOrder { get; init; } = 0;

        /// <summary>
        /// Value of org Config.
        /// </summary>
        [ProtoMember(7)]
        public string? Value { get; init; }

        /// <summary>
        /// The default value
        /// </summary>
        [ProtoMember(8)]
        public string? DefaultValue { get; init; }

        /// <summary>
        /// Minimum value of org Config.
        /// </summary>
        [ProtoMember(9)]
        public decimal? MinValue { get; init; }

        /// <summary>
        /// Maximum value of org Config.
        /// </summary>
        [ProtoMember(10)]
        public decimal? MaxValue { get; init; }

        /// <summary>
        /// Modifyed dateTimeUtc of org Config.
        /// </summary>
        [ProtoMember(11)]
        public DateTime? ModifyDateTimeUtc { get; init; }

        /// <summary>
        /// Modified user of org Config.
        /// </summary>
        [ProtoMember(12)]
        public string? ModifyUser { get; init; }

        /// <summary>
        /// The regular expression to use to validate the value
        /// </summary>
        [ProtoMember(13)]
        public string? RegexValidator { get; init; }

        /// <summary>
        /// List of field value of org Config.
        /// </summary>
        [ProtoMember(14)]
        public List<FieldValuesDto> FieldValues { get; init; } = new();

        public T? GetValue<T>()
        {
            var stringValue = Value ?? DefaultValue;
            if (stringValue is not null)
            {
                if (FieldType == FieldType.Json)
                {
                    JsonSerializerOptions? options = null;
                    return JsonSerializer.Deserialize<T>(stringValue, options);
                }
                else if (typeof(T).TryParse(stringValue, out var value))
                {
                    return (T?)value;
                }
            }
            return default;
        }
    }

    [Serializable]
    [ProtoContract]
    public record FieldValuesDto
    {
        /// <summary>
        /// Value of field value.
        /// </summary>
        [ProtoMember(1)]
        public string Value { get; init; } = "";

        /// <summary>
        /// Text of field value.
        /// </summary>
        [ProtoMember(2)]
        public string? Text { get; init; }

        /// <summary>
        /// Display order of field value.
        /// </summary>
        [ProtoMember(3)]
        public short DisplayOrder { get; init; }
    }
}
