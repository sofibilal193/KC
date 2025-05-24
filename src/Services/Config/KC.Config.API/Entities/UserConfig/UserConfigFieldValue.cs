using KC.Domain.Common.Entities;
using KC.Domain.Common.Events;

namespace KC.Config.API.Entities
{
    public class UserConfigFieldValue : SqlEntity
    {
        public int UserConfigFieldId { get; init; }

        public UserConfigField? OrgConfigField { get; init; }

        public int OrgId { get; set; }

        public int UserId { get; set; }

        public string? Value { get; private set; }

        #region Constructors

        public UserConfigFieldValue(int userConfigFieldId, int orgId, int userId, string? value)
        {
            UserConfigFieldId = userConfigFieldId;
            OrgId = orgId;
            UserId = userId;
            Value = value;
            AddDomainEvent(new EntityCreatedDomainEvent<UserConfigFieldValue>(this));
        }

        #endregion

        #region Methods

        public void Update(string? value)
        {
            Value = value;
            AddDomainEvent(new EntityUpdatedDomainEvent<UserConfigFieldValue>(this));
        }

        #endregion

    }
}
