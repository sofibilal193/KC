using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace KC.Infrastructure.Common.ServiceBus
{
    public interface IServiceBusPersisterConnection : IDisposable, IAsyncDisposable
    {
        ServiceBusClient TopicClient { get; }

        ServiceBusAdministrationClient AdministrationClient { get; }

        string GetTopicName(string eventName);

        ServiceBusSender? GetSender(string eventName);

        ServiceBusReceiver? GetReceiver(string eventName);

        ServiceBusProcessor GetProcessor(string topic);

        Task CreateTopicAsync(string topicName,
            CancellationToken cancellationToken = default);

        Task CreateSubscriptionAsync(string topicName,
            string subscriptionName, CancellationToken cancellationToken = default);

        Task CreateRuleAsync(string topicName, string subscriptionName,
            string ruleName, string subject, CancellationToken cancellationToken = default);
    }
}
