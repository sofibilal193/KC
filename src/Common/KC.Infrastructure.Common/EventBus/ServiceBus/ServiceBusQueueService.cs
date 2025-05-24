using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using KC.Domain.Common;
using KC.Domain.Common.Exceptions;
using KC.Infrastructure.Common.ServiceBus;

namespace KC.Infrastructure.Common.EventBus.ServiceBus
{
    public class ServiceBusQueueService : IQueueService
    {
        private readonly ServiceBusOptions _settings;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusAdministrationClient _adminClient;
        private readonly IDateTime _dt;
        private readonly ILogger<ServiceBusQueueService> _logger;
        private readonly Dictionary<string, ServiceBusSender> _senders = new();

        public ServiceBusQueueService(IServiceBusClientFactory clientFactory,
            IServiceBusAdministrativeClientFactory adminClientFactory, IOptions<InfraSettings> settings,
            IDateTime dt, ILogger<ServiceBusQueueService> logger)
        {
            _settings = settings.Value.ServiceBusSettings;
            _client = clientFactory.CreateClient(_settings.Namespace, new DefaultAzureCredential());
            _adminClient = adminClientFactory.CreateClient(_settings.Namespace, new DefaultAzureCredential());
            _dt = dt;
            _logger = logger;
        }

        public async Task<string> ScheduleMessageAsync(object message, TimeSpan delay, CancellationToken cancellationToken = default)
        {
            var messageType = message.GetType();
            if (_settings.Queues[messageType.Name].IsDisabled)
            {
                return $"Queue for {messageType} message is disabled.";
            }
            _logger.LogInformation("Scheduling {MessageType} message.", messageType.Name);
            var queueMessage = CreateQueueMessage(message);
            var sender = await GetSenderAsync(messageType, cancellationToken);
            await sender.ScheduleMessageAsync(queueMessage, _dt.Now.Add(delay), cancellationToken);
            return queueMessage.MessageId;
        }

        public async Task<string> SendMessageAsync(object message, CancellationToken cancellationToken = default)
        {
            var messageType = message.GetType();
            if (_settings.Queues[messageType.Name].IsDisabled)
            {
                return $"Queue for {messageType} message is disabled.";
            }
            _logger.LogInformation("Queueing {MessageType} message.", messageType.Name);
            var queueMessage = CreateQueueMessage(message);
            var sender = await GetSenderAsync(messageType, cancellationToken);
            await sender.SendMessageAsync(queueMessage, cancellationToken);
            return queueMessage.MessageId;
        }

        public async Task<string> SendMessagesAsync(IEnumerable<object> messages, CancellationToken cancellationToken = default)
        {
            var messageType = messages.FirstOrDefault()?.GetType();
            if (messageType is null || _settings.Queues[messageType.Name].IsDisabled)
            {
                return $"Queue for {messageType} message is disabled.";
            }
            _logger.LogInformation("Queueing {MessageType} messages.", messageType.Name);
            var sessionId = Guid.NewGuid().ToString();
            var messagesToQueue = new Queue<ServiceBusMessage>(messages.Select(m => CreateQueueMessage(m, sessionId)));
            var batches = new List<ServiceBusMessageBatch>();
            var index = 0;
            var sender = await GetSenderAsync(messageType, cancellationToken);
            while (messagesToQueue.Count > 0)
            {
                using var batch = await sender.CreateMessageBatchAsync(cancellationToken);
                var message = messagesToQueue.Peek();
                if (batch.TryAddMessage(message))
                {
                    messagesToQueue.Dequeue();
                    index++;
                }
                else
                {
                    throw new DomainException($"Message[{index}] is too large. No messages have been sent.");
                }
                while (messagesToQueue.Count > 0 && batch.TryAddMessage(messagesToQueue.Peek()))
                {
                    messagesToQueue.Dequeue();
                    index++;
                }
                batches.Add(batch);
            }

            foreach (var batch in batches)
            {
                await sender.SendMessagesAsync(batch, cancellationToken);
            }
            return sessionId;
        }

        private async Task<ServiceBusSender> GetSenderAsync(Type messageType, CancellationToken cancellationToken)
        {
            var queueName = _settings.Queues[messageType.Name].QueueName;
            var sender = _senders.GetValueOrDefault(queueName);
            if (sender is null)
            {
                sender = _client.CreateSender(queueName);
                await CreateQueueIfNotExistsAsync(queueName, cancellationToken);
                _senders.Add(queueName, sender);
            }
            return sender;
        }

        private async Task CreateQueueIfNotExistsAsync(string queueName, CancellationToken cancellationToken)
        {
            if (!await _adminClient.QueueExistsAsync(queueName, cancellationToken))
            {
                _logger.LogInformation("Creating service bus queue: {QueueName}.", queueName);
                var queue = new CreateQueueOptions(queueName)
                {
                    RequiresSession = true
                };
                await _adminClient.CreateQueueAsync(queue, cancellationToken);
            }
        }

        private ServiceBusMessage CreateQueueMessage(object message, string? sessionId = null)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var jsonMessage = JsonSerializer.Serialize(message, options);
            _logger.LogDebug("Message: {QueueMessage}", jsonMessage);
            return new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                MessageId = Guid.NewGuid().ToString(),
                SessionId = sessionId ?? Guid.NewGuid().ToString()
            };
        }
    }
}
