using MediatR;
using KC.Application.Common.IntegrationEvents;

namespace KC.Config.API.Application.Events
{
    public class OrgConfigFieldsUpdatedDomainEventHandler : INotificationHandler<OrgConfigFieldsUpdatedDomainEvent>
    {
        private readonly IIntegrationEventService _integrationService;

        public OrgConfigFieldsUpdatedDomainEventHandler(IIntegrationEventService integrationService)
        {
            _integrationService = integrationService;
        }

        public async Task Handle(OrgConfigFieldsUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            // fire integration event
            var integrationEvent = new OrgConfigFieldsUpdatedIntegrationEvent(notification);
            await _integrationService.AddAndSaveEventAsync(integrationEvent, cancellationToken);
        }
    }
}
