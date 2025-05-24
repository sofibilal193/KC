using KC.Domain.Common.Entities;
using KC.Domain.Common.Events;

namespace KC.Config.API.Entities
{
    public class OrgConfigFieldValue : SqlEntity
    {
        public int OrgConfigFieldId { get; init; }

        public OrgConfigField? OrgConfigField { get; init; }

        public int OrgId { get; set; }

        public string? Value { get; private set; }

        #region Constructors

        public OrgConfigFieldValue(int orgConfigFieldId, int orgId, string? value)
        {
            OrgConfigFieldId = orgConfigFieldId;
            OrgId = orgId;
            Value = value;
            AddDomainEvent(new EntityCreatedDomainEvent<OrgConfigFieldValue>(this));
        }

        #endregion

        #region Methods

        public void Update(string? value)
        {
            Value = value;
            AddDomainEvent(new EntityUpdatedDomainEvent<OrgConfigFieldValue>(this));
        }

        #endregion

    }
}
