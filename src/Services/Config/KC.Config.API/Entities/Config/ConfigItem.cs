using KC.Domain.Common.Entities;
using System.Text.Json.Serialization;

namespace KC.Config.API.Entities
{
    public class ConfigItem : SqlEntity
    {
        [JsonInclude]
        public string Type { get; init; } = "";

        [JsonInclude]
        public string Name { get; init; } = "";

        [JsonInclude]
        public string? Value { get; private set; }

        [JsonInclude]
        public string? Description { get; init; }

        [JsonInclude]
        public short DisplayOrder { get; init; } = 0;

        // indicates whether this item is private and should not be returned to UI clients
        [JsonInclude]
        public bool IsInternal { get; init; }

        [JsonInclude]
        public bool IsEncrypted { get; init; }

        #region Constructors

        [JsonConstructor]
        public ConfigItem() { }

        public ConfigItem(string type, string name, string value, bool isEncrypted)
        {
            Type = type;
            Name = name;
            Value = value;
            IsEncrypted = isEncrypted;
        }

        #endregion

        #region Methods

        public void SetValue(string? value)
        {
            Value = value;
        }

        #endregion

    }
}
