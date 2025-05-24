using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace KC.Infrastructure.Common.ServiceBus
{
    public class DefaultServiceBusPersisterConnection : IServiceBusPersisterConnection
    {
        private readonly ServiceBusOptions _options;
        private ServiceBusClient _topicClient;
        private readonly Dictionary<string, ServiceBusSender> _senders = new();
        private readonly Dictionary<string, ServiceBusReceiver> _receivers = new();
        private readonly Dictionary<string, ServiceBusProcessor> _processors = new();

        bool _disposed;

        public DefaultServiceBusPersisterConnection(ServiceBusOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            AdministrationClient = new ServiceBusAdministrationClient(
                _options.Namespace, new DefaultAzureCredential());
            _topicClient = new ServiceBusClient(
                _options.Namespace, new DefaultAzureCredential());
        }

        public async Task CreateTopicAsync(string topicName,
            CancellationToken cancellationToken = default)
        {
            if (!await AdministrationClient.TopicExistsAsync(topicName, cancellationToken))
            {
                await AdministrationClient.CreateTopicAsync(new CreateTopicOptions(topicName), cancellationToken);
            }
        }

        public async Task CreateSubscriptionAsync(string topicName,
            string subscriptionName, CancellationToken cancellationToken = default)
        {
            if (!await AdministrationClient.SubscriptionExistsAsync(topicName, subscriptionName, cancellationToken))
            {
                await AdministrationClient.CreateSubscriptionAsync(topicName, subscriptionName, cancellationToken);
                await AdministrationClient.DeleteRuleAsync(topicName, subscriptionName, RuleProperties.DefaultRuleName, cancellationToken);
            }
        }

        public async Task CreateRuleAsync(string topicName, string subscriptionName,
            string ruleName, string subject, CancellationToken cancellationToken = default)
        {
            if (!await AdministrationClient.RuleExistsAsync(
                topicName, _options.SubscriptionClientName, ruleName, cancellationToken))
            {
                await AdministrationClient.CreateRuleAsync(topicName,
                    _options.SubscriptionClientName, new CreateRuleOptions
                    {
                        Filter = new CorrelationRuleFilter() { Subject = subject },
                        Name = ruleName
                    }, cancellationToken);
            }
        }

        public ServiceBusClient TopicClient
        {
            get
            {
                if (_topicClient.IsClosed)
                {
                    _topicClient = new ServiceBusClient(_options.Namespace,
                        new DefaultAzureCredential());
                }
                return _topicClient;
            }
        }

        public ServiceBusAdministrationClient AdministrationClient { get; }

        public string GetTopicName(string eventName)
        {
            return eventName.Replace("IntegrationEvent", "").ToLowerInvariant();
        }

        public ServiceBusSender? GetSender(string eventName)
        {
            var topic = GetTopicName(eventName);
            if (!string.IsNullOrEmpty(topic))
            {
                if (_senders.TryGetValue(topic, out var sender))
                {
                    return sender;
                }
                return _topicClient.CreateSender(topic);
            }
            return null;
        }

        public ServiceBusReceiver? GetReceiver(string eventName)
        {
            var topic = GetTopicName(eventName);
            if (!string.IsNullOrEmpty(topic))
            {
                if (_receivers.TryGetValue(topic, out var receiver))
                {
                    return receiver;
                }
                return _topicClient.CreateReceiver(topic);
            }
            return null;
        }

        public ServiceBusProcessor GetProcessor(string topic)
        {
            if (_processors.TryGetValue(topic, out var processor))
            {
                return processor;
            }
            else
            {
                processor = _topicClient.CreateProcessor(topic, _options.SubscriptionClientName,
                new ServiceBusProcessorOptions
                {
                    MaxConcurrentCalls = _options.MaxConcurrentCalls,
                    AutoCompleteMessages = _options.AutoCompleteMessages
                });
                _processors.Add(topic, processor);
                return processor;
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

        protected virtual void Dispose(bool disposing)
        {
#pragma warning disable CA2012 //ValueTask instances returned from method calls should be directly awaited, returned, or passed as an argument to another method call. Other usage, such as storing an instance into a local or a field, is likely an indication of a bug, as ValueTask instances must only ever be consumed once.
            if (disposing && !_disposed)
            {
                // dispose resources
                foreach (var key in _senders.Keys)
                {
                    _senders[key].DisposeAsync().GetAwaiter();
                }
                foreach (var key in _receivers.Keys)
                {
                    _receivers[key].DisposeAsync().GetAwaiter();
                }
                foreach (var key in _processors.Keys)
                {
                    _processors[key].DisposeAsync().GetAwaiter();
                }
                _topicClient.DisposeAsync().GetAwaiter();

                _disposed = true;
            }
#pragma warning restore CA2012 //ValueTask instances returned from method calls should be directly awaited, returned, or passed as an argument to another method call. Other usage, such as storing an instance into a local or a field, is likely an indication of a bug, as ValueTask instances must only ever be consumed once.
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_disposed) return;

            // dispose resources
            foreach (var key in _senders.Keys)
            {
                await _senders[key].DisposeAsync().ConfigureAwait(false);
            }
            foreach (var key in _receivers.Keys)
            {
                await _receivers[key].DisposeAsync().ConfigureAwait(false);
            }
            foreach (var key in _processors.Keys)
            {
                await _processors[key].DisposeAsync().ConfigureAwait(false);
            }
            await _topicClient.DisposeAsync().ConfigureAwait(false);

            _disposed = true;
        }

        #endregion
    }
}
