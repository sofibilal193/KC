using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using KC.Application.Common.IntegrationEvents;
using KC.Domain.Common.Events;

namespace KC.Infrastructure.Common.ServiceBus
{
    public class EventBusServiceBus : IEventBus, IAsyncDisposable
    {
        private readonly IServiceBusPersisterConnection _serviceBusPersisterConnection;
        private readonly ILogger<EventBusServiceBus> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ServiceBusOptions _options;

        public EventBusServiceBus(ServiceBusOptions options, IServiceBusPersisterConnection serviceBusPersisterConnection,
            ILogger<EventBusServiceBus> logger, IEventBusSubscriptionsManager subsManager, IServiceProvider serviceProvider)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serviceBusPersisterConnection = serviceBusPersisterConnection ?? throw new ArgumentNullException(nameof(serviceBusPersisterConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync(IntegrationEvent integrationEvent)
        {
            var eventName = integrationEvent.GetType().Name;
            var jsonMessage = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType());
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            var sender = _serviceBusPersisterConnection.GetSender(eventName);

            if (sender is not null)
            {
                var message = new ServiceBusMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Body = new BinaryData(body),
                    Subject = eventName,
                };

                await sender.SendMessageAsync(message);
            }
        }

        public async Task SubscribeDynamicAsync<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                await SubscribeToTopicAsync(eventName);
            }
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}",
                eventName, typeof(TH).Name);
            _subsManager.AddDynamicSubscription<TH>(eventName);
        }

        public async Task SubscribeDynamicAsync(string eventName, Type type)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                await SubscribeToTopicAsync(eventName);
            }
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}",
                eventName, type.Name);
            _subsManager.AddDynamicSubscription(eventName, type);
        }

        public async Task SubscribeAsync<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var containsKey = _subsManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                await SubscribeToTopicAsync(eventName);
            }
            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);
            _subsManager.AddSubscription<T, TH>();
        }

        public async Task UnsubscribeAsync<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;
            await UnSubscribeFromTopicAsync(eventName);
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            _subsManager.RemoveSubscription<T, TH>();
        }

        public async Task UnsubscribeDynamicAsync<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            await UnSubscribeFromTopicAsync(eventName);
            _logger.LogInformation("Unsubscribing from dynamic event {EventName}", eventName);
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
        }

        public async Task UnsubscribeDynamicAsync(string eventName, Type type)
        {
            await UnSubscribeFromTopicAsync(eventName);
            _logger.LogInformation("Unsubscribing from dynamic event {EventName}", eventName);
            _subsManager.RemoveDynamicSubscription(eventName, type);
        }

        [ExcludeFromCodeCoverage]
        private async Task SubscribeToTopicAsync(string eventName)
        {
            var topicName = _serviceBusPersisterConnection.GetTopicName(eventName) ?? eventName;
            try
            {
                // await _serviceBusPersisterConnection.CreateTopicAsync(topicName);
                await _serviceBusPersisterConnection.CreateSubscriptionAsync(topicName, _options.SubscriptionClientName);

                await _serviceBusPersisterConnection.CreateRuleAsync(topicName, _options.SubscriptionClientName,
                    string.Concat("Rule.", eventName), eventName);
                await RegisterSubscriptionClientMessageHandlerAsync(topicName);
            }
            catch (ServiceBusException ex)
            {
                _logger.LogError(ex, "An error has occurred while subscribing to topic: {topic} for subscription: {sub} with event: {event}.",
                    topicName, _options.SubscriptionClientName, eventName);
            }
        }

        [ExcludeFromCodeCoverage]
        private async Task UnSubscribeFromTopicAsync(string eventName)
        {
            var topicName = _serviceBusPersisterConnection.GetTopicName(eventName);
            try
            {
                await _serviceBusPersisterConnection.AdministrationClient
                    .DeleteRuleAsync(topicName, _options.SubscriptionClientName, eventName);
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            {
                _logger.LogWarning("The messaging entity {eventName} Could not be found.", eventName);
            }
        }

        [ExcludeFromCodeCoverage]
        private async Task RegisterSubscriptionClientMessageHandlerAsync(string topicName)
        {
            var processor = _serviceBusPersisterConnection.GetProcessor(topicName);
            if (processor is not null)
            {
                processor.ProcessMessageAsync +=
                    async (args) =>
                    {
                        var eventName = args.Message.Subject;
                        string messageData = args.Message.Body.ToString();

                        // Complete the message so that it is not received again.
                        if (await ProcessEventAsync(eventName, messageData))
                        {
                            await args.CompleteMessageAsync(args.Message);
                        }
                    };

                processor.ProcessErrorAsync += ErrorHandler;
                await processor.StartProcessingAsync();
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
            if (disposing)
            {
                // dispose resources
                _subsManager.Clear();
            }
        }

        [ExcludeFromCodeCoverage]
        protected virtual ValueTask DisposeAsyncCore()
        {
            // dispose resources
            _subsManager.Clear();
            return ValueTask.CompletedTask;
        }

        #endregion

        [ExcludeFromCodeCoverage]
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            var ex = args.Exception;
            var context = args.ErrorSource;
            _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context);
            return Task.CompletedTask;
        }

        [ExcludeFromCodeCoverage]
        private async Task<bool> ProcessEventAsync(string eventName, string message)
        {
            var processed = false;
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.IsDynamic)
                        {
                            if (scope.ServiceProvider.GetRequiredService(subscription.HandlerType) is not IDynamicIntegrationEventHandler handler) continue;
                            using dynamic eventData = JsonDocument.Parse(message);
                            await handler.Handle(eventData);
                        }
                        else
                        {
                            var handler = scope.ServiceProvider.GetRequiredService(subscription.HandlerType);
                            if (handler == null) continue;
                            var eventType = _subsManager.GetEventTypeByName(eventName);
                            if (eventType is not null)
                            {
                                var integrationEvent = JsonSerializer.Deserialize(message, eventType);
                                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                                var task = concreteType?.GetMethod("Handle")?.Invoke(handler, new object?[] { integrationEvent });
                                if (task is not null)
                                {
                                    await (Task)task;
                                }
                            }
                        }
                    }
                }
                processed = true;
            }
            return processed;
        }

    }
}
