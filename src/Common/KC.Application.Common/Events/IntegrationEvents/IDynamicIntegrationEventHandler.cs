using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KC.Application.Common.IntegrationEvents
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(JsonDocument eventData, CancellationToken cancellationToken = default);
    }
}
