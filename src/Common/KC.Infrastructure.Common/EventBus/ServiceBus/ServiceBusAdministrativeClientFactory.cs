using Azure.Core;
using Azure.Messaging.ServiceBus.Administration;

namespace KC.Infrastructure.Common.EventBus.ServiceBus
{
    public interface IServiceBusAdministrativeClientFactory
    {
        ServiceBusAdministrationClient CreateClient(string fullyQualifiedNamespace, TokenCredential credential);
    }

    public class ServiceBusAdministrativeClientFactory : IServiceBusAdministrativeClientFactory
    {
        public ServiceBusAdministrationClient CreateClient(string fullyQualifiedNamespace, TokenCredential credential)
        {
            return new ServiceBusAdministrationClient(fullyQualifiedNamespace, credential);
        }
    }
}
