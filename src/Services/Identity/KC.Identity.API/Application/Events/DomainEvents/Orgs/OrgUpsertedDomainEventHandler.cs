using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using KC.Application.Common.IntegrationEvents;
using KC.Identity.API.DomainEvents;
using KC.Identity.API.IntegrationEvents;

namespace KC.Identity.API.Application.Events
{
    public class OrgUpsertedDomainEventHandler : INotificationHandler<OrgUpsertedDomainEvent>
    {
        private readonly IIntegrationEventService _integrationService;

        public OrgUpsertedDomainEventHandler(IIntegrationEventService integrationService)
        {
            _integrationService = integrationService ?? throw new ArgumentNullException(nameof(integrationService));
        }

        public async Task Handle(OrgUpsertedDomainEvent @event, CancellationToken cancellationToken)
        {
            if (@event is not null)
            {
                // fire integration event
                var integrationEvent = new OrgUpsertedIntegrationEvent(@event);
                await _integrationService.AddAndSaveEventAsync(integrationEvent, cancellationToken);
            }
        }
    }
}
