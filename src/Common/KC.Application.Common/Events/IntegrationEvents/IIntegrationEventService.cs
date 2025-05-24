using System;
using System.Threading;
using System.Threading.Tasks;
using KC.Domain.Common.Events;

namespace KC.Application.Common.IntegrationEvents
{
    public interface IIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(CancellationToken cancellationToken = default);

        Task PublishEventsThroughEventBusAsync(Guid transactionId,
            CancellationToken cancellationToken = default);
        Task AddAndSaveEventAsync(IntegrationEvent integrationEvent,
            CancellationToken cancellationToken = default);
    }
}
