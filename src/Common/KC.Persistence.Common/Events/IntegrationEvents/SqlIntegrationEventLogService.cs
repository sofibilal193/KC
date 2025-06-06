﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using KC.Application.Common.IntegrationEvents;
using KC.Application.Common.Repositories;
using KC.Domain.Common.Entities.IntegrationEvents;
using KC.Domain.Common.Events;
using KC.Persistence.Common.Events;

namespace KC.Persistence.Common.IntegrationEvents
{
    public class SqlIntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly EventSqlDbContext _context;
        private readonly List<Type> _eventTypes = new();
        private readonly List<string?> _eventTypeNames = new();
        private volatile bool _disposed;

        public IDbContext GetDbContext() => _context;

        public SqlIntegrationEventLogService(EventSqlDbContext context, params Assembly[] assemblies)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            if (assemblies?.Length > 0)
            {
                foreach (var assembly in assemblies)
                {
                    _eventTypes.AddRange(assembly
                        .GetTypes()
                        .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                        .ToList()
                    );
                    _eventTypeNames.AddRange(_eventTypes.Select(t => t.FullName));
                }
            }
        }

        public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await _context.IntegrationEventLogs
                .Where(e => (e.State == IntegrationEventState.NotPublished
                    || e.State == IntegrationEventState.InProgress)
                        && _eventTypeNames.Contains(e.EventTypeName))
                .ToListAsync(cancellationToken: cancellationToken);

            if (result?.Count > 0)
            {
                return result.OrderBy(o => o.CreateDateTimeUtc)
                    .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)!));
            }

            return new List<SqlIntegrationEventLog>();
        }

        [ExcludeFromCodeCoverage]
        public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(
            Guid transactionId, CancellationToken cancellationToken = default)
        {
            var result = await _context.IntegrationEventLogs
                .Where(e => e.TransactionId == transactionId &&
                    (e.State == IntegrationEventState.NotPublished
                        || e.State == IntegrationEventState.InProgress)
                            && _eventTypeNames.Contains(e.EventTypeName))
                .ToListAsync(cancellationToken: cancellationToken);

            if (result?.Count > 0)
            {
                return result.OrderBy(o => o.CreateDateTimeUtc)
                    .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)!));
            }

            return new List<SqlIntegrationEventLog>();
        }

        public async Task SaveEventAsync(IntegrationEvent integrationEvent, IDbContextTransaction? transaction = default,
            CancellationToken cancellationToken = default)
        {
            if (_context is null)
            {
                return;
            }

            if (transaction == null && _context.HasActiveTransaction)
                transaction = _context.GetCurrentTransaction()!;
            if (transaction != null && transaction != _context.GetCurrentTransaction())
                await _context.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

            var eventLog = new SqlIntegrationEventLog(integrationEvent, transaction?.TransactionId);

            _context.IntegrationEventLogs.Add(eventLog);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkEventsAsPublishedAsync(IList<Guid> events, CancellationToken cancellationToken = default)
        {
            await DeleteEventsAsync(events, cancellationToken);
        }

        public async Task MarkEventsAsFailedAsync(IList<Guid> events, CancellationToken cancellationToken = default)
        {
            var logs = await _context.IntegrationEventLogs
                .Where(e => events.Contains(e.EventId))
                .ToListAsync(cancellationToken: cancellationToken);

            foreach (var log in logs)
            {
                log.MarkFailed();
                _context.IntegrationEventLogs.Update(log);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task DeleteEventsAsync(IList<Guid> events, CancellationToken cancellationToken = default)
        {
            if (_context.IsSql)
            {
                await _context.IntegrationEventLogs.Where(e => events.Contains(e.EventId)).ExecuteDeleteAsync(cancellationToken);
            }
            else
            {
                var logs = await _context.IntegrationEventLogs
                    .Where(e => events.Contains(e.EventId))
                    .ToListAsync(cancellationToken: cancellationToken);
                if (logs?.Count > 0)
                {
                    _context.IntegrationEventLogs.RemoveRange(logs);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        [ExcludeFromCodeCoverage]
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose resources
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        [ExcludeFromCodeCoverage]
        protected virtual async ValueTask DisposeAsyncCore()
        {
            // dispose resources
            await _context.DisposeAsync().ConfigureAwait(false);
        }

        #endregion

    }
}
