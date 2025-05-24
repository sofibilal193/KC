using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KC.Application.Common.IntegrationEvents;
using KC.Domain.Common.Events;
using KC.Domain.Common.Extensions;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace KC.Infrastructure.Common.RabbitMQ
{
    [ExcludeFromCodeCoverage]
    public class EventBusRabbitMQ : IEventBus
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQOptions _options;
        private IModel _consumerChannel;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            IServiceProvider serviceProvider, IEventBusSubscriptionsManager subsManager, RabbitMQOptions options)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _serviceProvider = serviceProvider;
            _options = options;
            _consumerChannel = CreateConsumerChannel();
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved!;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using var channel = _persistentConnection.CreateModel();
            channel.QueueUnbind(queue: _options.QueueName,
                exchange: _options.BrokerName,
                routingKey: eventName);

            if (_subsManager.IsEmpty)
            {
                _consumerChannel.Close();
            }
        }

        public async Task PublishAsync(IntegrationEvent integrationEvent)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_options?.RetryCount ?? 5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})",
                    integrationEvent.Id, $"{time.TotalSeconds:n1}", ex.Message));

            var eventName = integrationEvent.GetType().Name;

            _logger.LogDebug("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", integrationEvent.Id, eventName);

            using var channel = _persistentConnection.CreateModel();
            _logger.LogDebug("Declaring RabbitMQ exchange to publish event: {EventId}", integrationEvent.Id);

            channel.ExchangeDeclare(exchange: _options?.BrokerName, type: "direct");

            var body = JsonSerializer.SerializeToUtf8Bytes(integrationEvent, integrationEvent.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                _logger.LogDebug("Publishing event to RabbitMQ: {EventId}", integrationEvent.Id);

                channel.BasicPublish(
                    exchange: _options?.BrokerName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
            await Task.CompletedTask;
        }

        public async Task SubscribeDynamicAsync<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}",
                eventName, typeof(TH).GetGenericTypeName());

            DoInternalSubscription(eventName);
            _subsManager.AddDynamicSubscription<TH>(eventName);
            StartBasicConsume();
            await Task.CompletedTask;
        }

        public async Task SubscribeDynamicAsync(string eventName, Type type)
        {
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}",
                eventName, type.GetGenericTypeName());

            DoInternalSubscription(eventName);
            _subsManager.AddDynamicSubscription(eventName, type);
            StartBasicConsume();
            await Task.CompletedTask;
        }

        public async Task SubscribeAsync<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}",
                eventName, typeof(TH).GetGenericTypeName());

            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume();
            await Task.CompletedTask;
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                _consumerChannel.QueueBind(queue: _options.QueueName,
                                    exchange: _options.BrokerName,
                                    routingKey: eventName);
            }
        }

        public async Task UnsubscribeAsync<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subsManager.RemoveSubscription<T, TH>();
            await Task.CompletedTask;
        }

        public async Task UnsubscribeDynamicAsync<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
            await Task.CompletedTask;
        }

        public async Task UnsubscribeDynamicAsync(string eventName, Type type)
        {
            _subsManager.RemoveDynamicSubscription(eventName, type);
            await Task.CompletedTask;
        }

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources
                _consumerChannel?.Dispose();
                _subsManager.Clear();
            }
        }

        #endregion

        private void StartBasicConsume()
        {
            _logger.LogDebug("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                if (_persistentConnection.IsAsyncConsumers)
                {
                    var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                    consumer.Received += Consumer_Async_Received;
                    _consumerChannel.BasicConsume(
                        queue: _options.QueueName,
                        autoAck: false,
                        consumer: consumer);
                }
                else
                {
                    var consumer = new EventingBasicConsumer(_consumerChannel);
                    consumer.Received += Consumer_Received!;
                    _consumerChannel.BasicConsume(
                        queue: _options.QueueName,
                        autoAck: false,
                        consumer: consumer);
                }
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                if (message.Contains("throw-fake-exception", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                ProcessEventAsync(eventName, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX).
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task Consumer_Async_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                if (message.Contains("throw-fake-exception", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await ProcessEventAsync(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX).
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogDebug("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: _options.BrokerName,
                                    type: "direct");

            channel.QueueDeclare(queue: _options.QueueName,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");
                _consumerChannel?.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private async Task ProcessEventAsync(string eventName, string message)
        {
            _logger.LogDebug("Processing RabbitMQ event: {EventName}", eventName);

            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using var scope = _serviceProvider?.CreateScope();
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        if (scope?.ServiceProvider.GetRequiredService(subscription.HandlerType) is not IDynamicIntegrationEventHandler handler) continue;
                        using dynamic eventData = JsonDocument.Parse(message);
                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        var handler = scope?.ServiceProvider.GetRequiredService(subscription.HandlerType);
                        if (handler is null) continue;
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        if (eventType is not null)
                        {
                            var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                            await Task.Yield();
                            var task = concreteType?.GetMethod("Handle")?.Invoke(handler, new object?[] { integrationEvent });
                            if (task is not null)
                            {
                                await (Task)task;
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            }
        }
    }
}
