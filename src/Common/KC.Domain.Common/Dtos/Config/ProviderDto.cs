using KC.Domain.Common.Config;
using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public record ProviderDto
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
        /// Tags for Provider.
        /// </summary>
        [ProtoMember(6)]
        public IList<TagDto>? Tags { get; init; }

        /// <summary>
        /// Orgs enabled for provider.
        /// </summary>
        [ProtoMember(7)]
        public IList<ProviderOrgDto> Orgs { get; init; } = new List<ProviderOrgDto>();

        public int? GetOrgId(string providerOrgId) =>
            Orgs.FirstOrDefault(o => o.ProviderOrgId == providerOrgId)?.OrgId;

        public string? GetProviderOrgId(int orgId) =>
            Orgs.FirstOrDefault(o => o.OrgId == orgId)?.ProviderOrgId;
    }
}
