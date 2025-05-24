using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using KC.Application.Common.IntegrationEvents;
using KC.Domain.Common.Events;
using KC.Domain.Common.Identity;
using KC.Identity.API.Entities;
using KC.Identity.API.IntegrationEvents;

namespace KC.Identity.API.Application.Events
{
    public class RoleCreatedDomainEventHandler : INotificationHandler<EntityCreatedDomainEvent<Role>>
    {
        private readonly IIntegrationEventService _integrationService;

        public RoleCreatedDomainEventHandler(IIntegrationEventService integrationService)
        {
            _integrationService = integrationService ?? throw new ArgumentNullException(nameof(integrationService));
        }

        public async Task Handle(EntityCreatedDomainEvent<Role> @event, CancellationToken cancellationToken)
        {
            var role = @event.Entity;
            if (role is not null)
            {
                // fire integration event
                @event.SetOrg(@event.Entity?.OrgId);
                var integrationEvent = new RoleCreatedIntegrationEvent(@event);
                await _integrationService.AddAndSaveEventAsync(integrationEvent, cancellationToken);
            }
        }
    }
}
