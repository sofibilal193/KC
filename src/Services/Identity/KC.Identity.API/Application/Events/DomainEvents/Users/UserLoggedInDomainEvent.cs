using KC.Domain.Common.Events;

namespace KC.Identity.API.DomainEvents
{
    /// <summary>
    /// Event used when a user logs in
    /// </summary>
    public record UserLoggedInDomainEvent
         : DomainEvent
    {
        public dynamic Id { get; }

        public UserLoggedInDomainEvent(dynamic id)
        {
            Id = id;
        }

        public UserLoggedInDomainEvent(dynamic id, dynamic? eventOrgId)
        {
            Id = id;
            SetOrg(eventOrgId);
        }
    }
}
