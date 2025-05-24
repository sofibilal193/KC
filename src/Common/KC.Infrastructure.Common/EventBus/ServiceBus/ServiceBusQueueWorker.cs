using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
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
    [ExcludeFromCodeCoverage]
    internal sealed class ServiceBusQueueWorker<TMessage> : IHostedService, IAsyncDisposable
    {
        private readonly ServiceBusQueueSettings _settings;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusAdministrationClient _adminClient;
        private readonly ServiceBusSender _sender;
        private readonly ServiceBusSessionProcessor _processor;
        private readonly IQueueProcessor<TMessage> _messageProcessor;
        private readonly IDateTime _dt;
        private readonly ILogger<ServiceBusQueueWorker<TMessage>> _logger;
        private bool _isFinalTry;

        public ServiceBusQueueWorker(IServiceBusClientFactory clientFactory,
            IServiceBusAdministrativeClientFactory adminClientFactory, IOptions<InfraSettings> settings,
            IQueueProcessor<TMessage> processor, IDateTime dt, ILogger<ServiceBusQueueWorker<TMessage>> logger)
        {
            var serviceBusSettings = settings.Value.ServiceBusSettings;
            _settings = serviceBusSettings.Queues[typeof(TMessage).Name];
            _client = clientFactory.CreateClient(serviceBusSettings.Namespace, new DefaultAzureCredential());
            _adminClient = adminClientFactory.CreateClient(serviceBusSettings.Namespace, new DefaultAzureCredential());
            _sender = _client.CreateSender(_settings.QueueName); // used for retries
            _processor = CreateProcessor();
            _messageProcessor = processor;
            _dt = dt;
            _logger = logger;
            _isFinalTry = _settings.MaxRetryCount == 0;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!await _adminClient.QueueExistsAsync(_settings.QueueName, cancellationToken))
                {
                    _logger.LogInformation("Creating service bus queue: {QueueName}.", _settings.QueueName);
                    var queue = new CreateQueueOptions(_settings.QueueName)
                    {
                        RequiresSession = true
                    };
                    await _adminClient.CreateQueueAsync(queue, cancellationToken);
                }
                _logger.LogInformation("Starting QueueWorker Service for processing messages of type: "
                    + "{QueueMessageType} from Service Bus Queue: {QueueName}.", typeof(TMessage).Name, _settings.QueueName);
                await _processor.StartProcessingAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting service.");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping service.");
            if (!_sender.IsClosed)
            {
                await _sender.CloseAsync(cancellationToken);
            }
            if (_processor.IsProcessing)
            {
                await _processor.StopProcessingAsync(cancellationToken);
            }
            if (!_processor.IsClosed)
            {
                await _processor.CloseAsync(cancellationToken);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
            await _processor.DisposeAsync();
            await _client.DisposeAsync();
        }

        private ServiceBusSessionProcessor CreateProcessor()
        {
            var options = new ServiceBusSessionProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentSessions = _settings.MaxConcurrentCalls,
            };
            var processor = _client.CreateSessionProcessor(_settings.QueueName, options);
            processor.ProcessMessageAsync += ProcessMessageAsync;
            processor.ProcessErrorAsync += ProcessErrorAsync;
            return processor;
        }

        private async Task ProcessMessageAsync(ProcessSessionMessageEventArgs args)
        {
            try
            {
                var messageBody = DeserializeMessage(args.Message);
                if (messageBody is not null)
                {
                    await _messageProcessor.ProcessMessageAsync(messageBody, _isFinalTry);
                    await args.CompleteMessageAsync(args.Message);
                }
            }
            catch (Exception ex)
            {
                if (ex is RetryableException rex)
                {
                    await RequeueMessageAsync(args, rex);
                }
                else
                {
                    // log error and move message to DeadLetter queue
                    _logger.LogError(ex, "Error processing message. " +
                        "This message will be sent to the deadletter queue (Message ID: {MessageId})", args.Message.MessageId);
                    await args.DeadLetterMessageAsync(args.Message, "Processing Exception", ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        private TMessage? DeserializeMessage(ServiceBusReceivedMessage message)
        {
            TMessage? messageBody;
            try
            {
                var messageBodyJson = Encoding.UTF8.GetString(message.Body);
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

        private async Task RequeueMessageAsync(ProcessSessionMessageEventArgs args, RetryableException ex)
        {
            var exception = ex.InnerException ?? ex;
            var retryCount = args.Message.ApplicationProperties.TryGetValue("RetryCount", out var value) ? (int)value : 0;
            int maxRetryCount = ex.MaxRetryCount ?? _settings.MaxRetryCount;
            if (retryCount >= maxRetryCount)
            {
                // move message to DeadLetter queue
                _logger.LogError(exception, "Error processing message. " +
                    "The maximum number of retries ({MaxRetryCount}) has been reached. " +
                    "This message will be sent to the deadletter queue (Message ID: {MessageId}).",
                    maxRetryCount, args.Message.MessageId);
                await args.DeadLetterMessageAsync(args.Message, "Too many retries", exception.Message);
            }
            else
            {
                // re-enqueue message
                var retryDelay = ex.GetRetryDelay(retryCount) ?? _settings.GetRetryDelay(retryCount);
                _logger.LogError(exception, "Error processing message (Attempt {AttemptCount} of {MaxAttemptCount}). " +
                    "Scheduling to retry in {RetryDelay} seconds (Message ID: {MessageId}).",
                    retryCount + 1, maxRetryCount + 1, retryDelay.TotalSeconds, args.Message.MessageId);
                var message = new ServiceBusMessage(args.Message);
                message.ApplicationProperties["RetryCount"] = retryCount + 1;
                await _sender.ScheduleMessageAsync(message, _dt.Now.Add(retryDelay));
                await args.CompleteMessageAsync(args.Message);
                _isFinalTry = retryCount == maxRetryCount - 1;
            }
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception,
                "Error communicating with service bus. (Queue: {EntityPath}, Source: {ErrorSource}).",
                args.EntityPath, args.ErrorSource);
            return Task.CompletedTask;
        }
    }
}
