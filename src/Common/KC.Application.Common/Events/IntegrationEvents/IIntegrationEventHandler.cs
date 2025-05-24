using System.Threading;
using System.Threading.Tasks;
using KC.Domain.Common.Events;

namespace KC.Application.Common.IntegrationEvents
{
    public interface IIntegrationEventHandler<in TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    }
}
