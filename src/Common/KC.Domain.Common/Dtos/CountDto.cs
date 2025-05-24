using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public record CountDto
    {
        /// <summary>
        /// Number of records.
        /// </summary>
        public int Count { get; init; }
    }
}
