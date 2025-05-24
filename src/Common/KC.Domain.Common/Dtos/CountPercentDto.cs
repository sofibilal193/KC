using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public record CountPercentDto
    {
        /// <summary>
        /// Number of records.
        /// </summary>
        [ProtoMember(1)]
        public int Count { get; init; }

        /// <summary>
        /// Calculated percent.
        /// </summary>
        [ProtoMember(2)]
        public decimal Percent { get; init; }
    }
}
