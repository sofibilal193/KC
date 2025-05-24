using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KC.Infrastructure.Common.RabbitMQ;
using KC.Infrastructure.Common.ServiceBus;
using RabbitMQ.Client;

namespace KC.Infrastructure.Common.EventBus.RabbitMQ
{
    /// <summary>
    /// Basic queue service using Rabbit MQ (Use for local development only)
    /// </summary>
    public class RabbitMQQueueService : IQueueService
    {
        private readonly IRabbitMQPersistentConnection _connection;
        private readonly ServiceBusOptions _settings;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQQueueService> _logger;

        public RabbitMQQueueService(IOptions<InfraSettings> settings,
            IRabbitMQPersistentConnection connection, ILogger<RabbitMQQueueService> logger)
        {
            _connection = connection;
            //_queueName = settings.Value.ServiceBusSettings.Queues[typeof(TMessage).Name].QueueName;
            _settings = settings.Value.ServiceBusSettings;
            _logger = logger;

            connection.TryConnect();
            _channel = connection.CreateModel();
        }

        public async Task<string> ScheduleMessageAsync(object message, TimeSpan delay, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("RabbitMQ does not support message scheduling. Message will be send immediately.");
            return await SendMessageAsync(message, cancellationToken);
        }

        public Task<string> SendMessageAsync(object message, CancellationToken cancellationToken = default)
        {
            var messageType = message.GetType().Name;
            var queueName = _settings.Queues[messageType].QueueName;
            _logger.LogInformation("Queueing {MessageType} message.", messageType);
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }
            var queueMessage = CreateQueueMessage(message);
            var properties = _channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent
            properties.MessageId = Guid.NewGuid().ToString();
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.BasicPublish("", queueName, true, properties, queueMessage);
            return Task.FromResult(properties.MessageId);
        }

        public async Task<string> SendMessagesAsync(IEnumerable<object> messages, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("Messages may not be processed in order if multiple queue processor services are running.");
            var messageIds = new List<string>();
            foreach (var message in messages)
            {
                messageIds.Add(await SendMessageAsync(message, cancellationToken));
            }
            return string.Join(',', messageIds);
        }

        private byte[] CreateQueueMessage(object message)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var jsonMessage = JsonSerializer.Serialize(message, options);
            _logger.LogDebug("Message: {QueueMessage}", jsonMessage);
            return Encoding.UTF8.GetBytes(jsonMessage);
        }
    }
}
