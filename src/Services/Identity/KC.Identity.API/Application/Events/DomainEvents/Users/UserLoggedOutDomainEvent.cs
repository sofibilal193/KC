using KC.Domain.Common.Events;

namespace KC.Identity.API.DomainEvents
{
    /// <summary>
    /// Event used when a user logs out
    /// </summary>
    public record UserLoggedOutDomainEvent
         : DomainEvent
    {
        public dynamic Id { get; }

        public UserLoggedOutDomainEvent(dynamic id)
        {
            Id = id;
        }

        public UserLoggedOutDomainEvent(dynamic id, dynamic? eventOrgId)
        {
            Id = id;
            SetOrg(eventOrgId);
        }
    }
}
