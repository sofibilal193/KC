using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KC.Domain.Common.Exceptions;
using KC.Infrastructure.Common.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KC.Infrastructure.Common.EventBus.RabbitMQ
{
    [ExcludeFromCodeCoverage]
    internal sealed class RabbitMQQueueWorker<TMessage> : IHostedService, IAsyncDisposable
    {
        private readonly IRabbitMQPersistentConnection _connection;
        private readonly string _queueName;
        private readonly IQueueProcessor<TMessage> _processor;
        private readonly ILogger<RabbitMQQueueWorker<TMessage>> _logger;
        private IModel? _channel;

        public RabbitMQQueueWorker(IOptions<InfraSettings> settings, IRabbitMQPersistentConnection connection,
            IQueueProcessor<TMessage> processor, ILogger<RabbitMQQueueWorker<TMessage>> logger)
        {
            _connection = connection;
            _queueName = settings.Value.ServiceBusSettings.Queues[typeof(TMessage).Name].QueueName;
            _processor = processor;
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting QueueWorker Service for processing messages of type: "
                + "{QueueMessageType} from Rabbit MQ Queue: {QueueName}.", typeof(TMessage).Name, _queueName);

            _connection.TryConnect();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_queueName, true, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (_, args) =>
            {
                try
                {
                    var messageBody = DeserializeMessage(args.Body);
                    if (messageBody is not null)
                    {
                        await _processor.ProcessMessageAsync(messageBody, true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message (Message ID: {MessageId}).",
                        args.BasicProperties.MessageId);
                }
                _channel.BasicAck(args.DeliveryTag, false);
            };
            _channel.BasicConsume(_queueName, false, _queueName, consumer);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.BasicCancel(_queueName);
            _channel?.Close();
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            _channel?.Dispose();
            _connection.Dispose();
            return ValueTask.CompletedTask;
        }

        private TMessage? DeserializeMessage(ReadOnlyMemory<byte> message)
        {
            TMessage? messageBody;
            try
            {
                var messageBodyJson = Encoding.UTF8.GetString(message.Span);
                _logger.LogDebug("Message received: '{Message}'.", messageBodyJson);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                messageBody = JsonSerializer.Deserialize<TMessage>(messageBodyJson, options);
                if (messageBody is null)
                {
                    throw new DomainException("Message body is null.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing queue message to {MessageType}.", typeof(TMessage));
                throw;
            }
            return messageBody;
        }
    }
}
