using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using KC.Application.Common.Repositories;
using KC.Domain.Common.Entities.IntegrationEvents;
using KC.Domain.Common.Events;

namespace KC.Application.Common.IntegrationEvents
{
    public interface IIntegrationEventLogService : IDisposable, IAsyncDisposable
    {
        IDbContext GetDbContext();

        Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(
            CancellationToken cancellationToken = default);

        Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(
            Guid transactionId, CancellationToken cancellationToken = default);

        Task SaveEventAsync(IntegrationEvent integrationEvent, IDbContextTransaction? transaction = default,
            CancellationToken cancellationToken = default);

        Task MarkEventsAsPublishedAsync(IList<Guid> events, CancellationToken cancellationToken = default);

        Task MarkEventsAsFailedAsync(IList<Guid> events, CancellationToken cancellationToken = default);
    }
}
