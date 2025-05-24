using KC.Domain.Common.Config;
using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public record OrgProviderDto
    {
        /// <summary>
        /// Id of Provider.
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; init; }

        /// <summary>
        /// Type of Provider.
        /// </summary>
        [ProtoMember(2)]
        public ProviderType Type { get; init; }

        /// <summary>
        /// Code of Provider.
        /// </summary>
        [ProtoMember(3)]
        public string Code { get; init; } = "";

        /// <summary>
        /// Name of Provider.
        /// </summary>
        [ProtoMember(4)]
        public string Name { get; init; } = "";

        /// <summary>
        /// Description of Provider.
        /// </summary>
        [ProtoMember(5)]
        public string? Description { get; init; }

        /// <summary>
        /// Provider's Id for the org/dealer
        /// </summary>
        [ProtoMember(6)]
        public string? ProviderOrgId { get; init; }

        /// <summary>
        /// Flag to determine if dealer is enabled for provider or not.
        /// </summary>
        [ProtoMember(7)]
        public bool IsEnabled { get; init; }

        /// <summary>
        /// Date/time in UTC time the record was last modified.
        /// </summary>
        [ProtoMember(8)]
        public DateTime? ModifyDateTimeUtc { get; init; }

        /// <summary>
        /// Name of user who last modified the record.
        /// </summary>
        [ProtoMember(9)]
        public string? ModifyUserName { get; init; }

        /// <summary>
        /// Id of Provider Org in Config API
        /// </summary>
        [ProtoMember(10)]
        public int? ConfigProviderOrgId { get; init; }

        /// <summary>
        /// Tags for Provider.
        /// </summary>
        [ProtoMember(11)]
        public IList<TagDto>? Tags { get; init; }
    }
}
