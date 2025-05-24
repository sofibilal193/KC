using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public record ProviderOrgDto
    {
        /// <summary>
        /// Id of Org.
        /// </summary>
        [ProtoMember(1)]
        public int OrgId { get; init; }

        /// <summary>
        /// Provider's Id for the org/dealer
        /// </summary>
        [ProtoMember(2)]
        public string? ProviderOrgId { get; init; }
    }
}
