using KC.Domain.Common.Config;
using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public record OrgProviderAccountDto
    {
        /// <summary>
        /// Id of Provider.
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; init; }

        /// <summary>
        /// Provider's Id for the org/dealer
        /// </summary>
        [ProtoMember(2)]
        public string? ProviderOrgId { get; init; }

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
        /// The Username or Account for the Org in the Provider's System
        /// </summary>
        [ProtoMember(5)]
        public string? Account { get; init; }

        /// <summary>
        /// The Password for the Org in the Provider's System
        /// </summary>
        [ProtoMember(6)]
        public string? Password { get; init; }

        /// <summary>
        /// Flag to determine if Org is enabled for provider or not.
        /// </summary>
        [ProtoMember(7)]
        public bool IsEnabled { get; init; }

        /// <summary>
        /// Id of Provider Org in Config API
        /// </summary>
        [ProtoMember(8)]
        public int? ConfigProviderOrgId { get; init; }
    }
}
