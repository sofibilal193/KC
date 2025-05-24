using KC.Domain.Common.Config;
using KC.Domain.Common.Identity;
using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public record OrgInfoDto
    {
        /// <summary>
        /// Name of the organization.
        /// </summary>
        [ProtoMember(1)]
        public string? Name { get; init; }

        /// <summary>
        /// Type of the organization.
        /// </summary>
        [ProtoMember(2)]
        public OrgType? Type { get; init; }

        /// <summary>
        /// Address of the organization.
        /// </summary>
        [ProtoMember(3)]
        public string? FullAddress { get; init; }

        /// <summary>
        /// Website address of the organization.
        /// </summary>
        [ProtoMember(4)]
        public string? Website { get; init; }

        /// <summary>
        /// Phone number of the organization.
        /// </summary>
        [ProtoMember(5)]
        public string? Phone { get; init; }

        /// <summary>
        /// List of field configs of the organization.
        /// </summary>
        [ProtoMember(6)]
        public List<OrgConfigDto>? Configs { get; set; }

        public void SetConfigs(List<OrgConfigDto>? configs)
        {
            Configs = configs;
        }

        public string? Lenders
        {
            get
            {
                return Configs?.Find(x => x.Name == nameof(FieldName.LenderList))?.GetValue<string>();
            }
        }

        public string? PrivacyPolicy
        {
            get
            {
                return Configs?.Find(x => x.Name == nameof(FieldName.PrivacyPolicy))?.GetValue<string>();
            }
        }

        public string? TermsOfUse
        {
            get
            {
                return Configs?.Find(x => x.Name == nameof(FieldName.TermsOfUse))?.GetValue<string>();
            }
        }

        public string? Logo
        {
            get
            {
                return Configs?.Find(x => x.Name == nameof(FieldName.Logo))?.GetValue<string>();
            }
        }

        public bool ShowMenuProductCost
        {
            get
            {
                return Configs?.Find(x => x.Name == nameof(FieldName.ShowProductCost))?.GetValue<bool>() ?? true;
            }
        }

        public bool ShowMenuProductPayments
        {
            get
            {
                return Configs?.Find(x => x.Name == nameof(FieldName.ShowPayments))?.GetValue<bool>() ?? true;
            }
        }
    }
}
