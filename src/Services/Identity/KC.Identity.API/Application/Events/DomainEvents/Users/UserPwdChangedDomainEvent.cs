using KC.Domain.Common.Events;

namespace KC.Identity.API.DomainEvents
{
    /// <summary>
    /// Event used when a user changes their password
    /// </summary>
    public record UserPwdChangedDomainEvent
         : DomainEvent
    {
        public string Email { get; init; }

        public UserPwdChangedDomainEvent(int userId, int? orgId, string name, string email)
        {
            SetUserSource(userId, name);
            SetOrg(orgId);
            Email = email;
        }
    }
}
