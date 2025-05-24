using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KC.Domain.Common.Events;
using KC.Utils.Common;

namespace KC.Application.Common.IntegrationEvents
{
    public class IntegrationEventService : IIntegrationEventService, IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<IntegrationEventService> _logger;
        private readonly SemaphoreSlim _semaphore = new(initialCount: 1, maxCount: 1); // https://kendaleiv.com/limiting-concurrent-operations-with-semaphoreslim-using-csharp/
        private const int _semaphoreWaitPeriodSeconds = 5;

        public IntegrationEventService(IEventBus eventBus,
            IIntegrationEventLogService eventLogService,
            ILogger<IntegrationEventService> logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphore.WaitAsync(TimeSpan.FromSeconds(_semaphoreWaitPeriodSeconds), cancellationToken);
            }
            catch
            {
                // We failed to enter the semaphore in the given amount of time. give up and return
                return;
            }

            try
            {
                List<Guid> publishedEvents = new(), failedEvents = new();
                var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(cancellationToken);
                foreach (var logEvt in pendingLogEvents)
                {
                    try
                    {
                        _logger.LogDebug("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})"
                            , logEvt.EventId, ReflectionUtil.GetAppName(), logEvt.Event);

                        if (logEvt.Event is not null)
                        {
                            await _eventBus.PublishAsync(logEvt.Event);
                            publishedEvents.Add(logEvt.EventId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}"
                            , logEvt.EventId, ReflectionUtil.GetAppName());
                        failedEvents.Add(logEvt.EventId);

                    }
                }

                if (publishedEvents.Count > 0)
                    await _eventLogService.MarkEventsAsPublishedAsync(publishedEvents, cancellationToken);
                if (failedEvents.Count > 0)
                    await _eventLogService.MarkEventsAsFailedAsync(failedEvents, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PublishEventsThroughEventBusAsync() ERROR.");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphore.WaitAsync(TimeSpan.FromSeconds(_semaphoreWaitPeriodSeconds), cancellationToken);
            }
            catch
            {
                // We failed to enter the semaphore in the given amount of time. give up and return
                return;
            }

            try
            {
                List<Guid> publishedEvents = new(), failedEvents = new();
                var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId, cancellationToken);

                foreach (var logEvt in pendingLogEvents)
                {
                    _logger.LogDebug("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})"
                        , logEvt.EventId, ReflectionUtil.GetAppName(), logEvt.Event);

                    try
                    {
                        if (logEvt.Event is not null)
                        {
                            await _eventBus.PublishAsync(logEvt.Event);
                            publishedEvents.Add(logEvt.EventId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}"
                            , logEvt.EventId, ReflectionUtil.GetAppName());
                        failedEvents.Add(logEvt.EventId);
                    }
                }

                if (publishedEvents.Count > 0)
                    await _eventLogService.MarkEventsAsPublishedAsync(publishedEvents, cancellationToken);
                if (failedEvents.Count > 0)
                    await _eventLogService.MarkEventsAsFailedAsync(failedEvents, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PublishEventsThroughEventBusAsync() ERROR.");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent integrationEvent,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})"
                , integrationEvent.Id, integrationEvent);
            var transaction = _eventLogService.GetDbContext()?.GetCurrentTransaction();
            await _eventLogService.SaveEventAsync(integrationEvent, transaction, cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _semaphore.Dispose();
        }
    }
}
