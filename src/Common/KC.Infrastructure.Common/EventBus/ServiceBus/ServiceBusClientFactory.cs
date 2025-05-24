using Azure.Core;
using Azure.Messaging.ServiceBus;

namespace KC.Infrastructure.Common.EventBus.ServiceBus
{
    public interface IServiceBusClientFactory
    {
        ServiceBusClient CreateClient(string fullyQualifiedNamespace, TokenCredential credential);
    }

    public class ServiceBusClientFactory : IServiceBusClientFactory
    {
        public ServiceBusClient CreateClient(string fullyQualifiedNamespace, TokenCredential credential)
        {
            return new ServiceBusClient(fullyQualifiedNamespace, credential);
        }
    }
}
