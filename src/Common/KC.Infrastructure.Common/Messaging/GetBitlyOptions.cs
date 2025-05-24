using KC.Infrastructure.Common;
using Microsoft.Extensions.Options;

namespace KC.Infrastructure.Common.Messaging
{
    public record GetBitlyOptions : BitlyOptions
    {
        public GetBitlyOptions(IOptionsMonitor<InfraSettings> settingsAccessor)
            : base(settingsAccessor.CurrentValue.BitlySettings)
        {
        }
    }
}
