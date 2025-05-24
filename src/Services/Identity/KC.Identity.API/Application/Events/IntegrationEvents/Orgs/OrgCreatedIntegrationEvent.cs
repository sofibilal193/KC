using KC.Domain.Common.Events;

namespace KC.Identity.API.IntegrationEvents
{
    public record OrgCreatedIntegrationEvent(int OrgId, string? UserEmail) : IntegrationEvent;
}
