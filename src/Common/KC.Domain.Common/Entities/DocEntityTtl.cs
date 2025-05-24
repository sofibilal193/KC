using System.Text.Json.Serialization;

namespace KC.Domain.Common.Entities
{
    public abstract class DocEntityTtl : DocEntity
    {
        [JsonPropertyName("ttl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? TimeToLive { get; private set; } = default;

        public void SetTTL(int ttl)
        {
            TimeToLive = ttl;
        }
    }
}
