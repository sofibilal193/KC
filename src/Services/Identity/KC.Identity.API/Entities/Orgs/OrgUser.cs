using KC.Domain.Common.Entities;
using KC.Domain.Common.Events;

namespace KC.Identity.API.Entities
{
    public class OrgUser : SqlEntity
    {
        public int OrgId { get; init; }

        public Org? Org { get; init; }

        public int UserId { get; init; }

        public User? User { get; init; }

        public int RoleId { get; set; }

        public Role? Role { get; set; }

        public string? Title { get; private set; }

        public bool IsDefault { get; private set; }

        public bool IsInvited { get; private set; }

        public bool IsInviteProcessed { get; private set; }

        protected OrgUser() : base() { }

        public OrgUser(User user, int orgId, int roleId,
            bool isDefault, string? title) : this()
        {
            User = user;
            OrgId = orgId;
            RoleId = roleId;
            IsDefault = isDefault;
            Title = title;
            if (user.Id == 0)
            {
                AddDomainEvent(new EntityCreatedDomainEvent<User>(user));
            }
        }

        public void Update(int roleId, bool isDefault, string? title)
        {
            RoleId = roleId;
            IsDefault = isDefault;
            Title = title;
        }

        public void Update(int roleId, string? title)
        {
            RoleId = roleId;
            Title = title;
        }

        public void Invite()
        {
            IsInvited = true;
        }

        public void ProcessInvite()
        {
            if (IsInvited && !IsInviteProcessed)
                IsInviteProcessed = true;
        }
    }
}
