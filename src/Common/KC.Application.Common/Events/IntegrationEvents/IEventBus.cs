using System;
using System.Threading.Tasks;
using KC.Domain.Common.Events;

namespace KC.Application.Common.IntegrationEvents
{
    public interface IEventBus : IDisposable
    {
        Task PublishAsync(IntegrationEvent integrationEvent);

        Task SubscribeAsync<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        Task SubscribeDynamicAsync<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        Task SubscribeDynamicAsync(string eventName, Type type);

        Task UnsubscribeDynamicAsync<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        Task UnsubscribeDynamicAsync(string eventName, Type type);

        Task UnsubscribeAsync<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}
