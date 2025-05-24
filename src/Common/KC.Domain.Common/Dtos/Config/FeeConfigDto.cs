using ProtoBuf;

namespace KC.Domain.Common.Config
{
    [Serializable]
    [ProtoContract]
    public readonly record struct FeeConfigDto
    {
        /// <summary>
        /// Type of fee.
        /// </summary>
        [ProtoMember(1)]
        public FeeType Type { get; init; }

        /// <summary>
        /// Type of deal.
        /// </summary>
        [ProtoMember(2)]
        public DealType? DealType { get; init; }

        /// <summary>
        /// Name of fee.
        /// </summary>
        [ProtoMember(3)]
        public string Name { get; init; } = "";

        /// <summary>
        /// Description of fee.
        /// </summary>
        [ProtoMember(4)]
        public string? Description { get; init; }

        /// <summary>
        /// Amount of fee.
        /// </summary>
        [ProtoMember(5)]
        public decimal Amount { get; init; }

        /// <summary>
        /// DMS mapping.
        /// </summary>
        [ProtoMember(6)]
        public string? DMSMapping { get; init; }

        /// <summary>
        /// Name of user who last modified the fee.
        /// </summary>
        [ProtoMember(7)]
        public string? LastModifiedUserName { get; init; }

        /// <summary>
        /// Last modified date of fee in UTC time.
        /// </summary>
        [ProtoMember(8)]
        public DateTime? LastModifiedDateTimeUtc { get; init; }

        public FeeConfigDto()
        {
        }
    }
}
