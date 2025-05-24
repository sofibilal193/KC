using Microsoft.Extensions.Options;

namespace KC.Infrastructure.Common.Messaging
{
    public record GetTwilioClientOptions : TwilioClientOptions
    {
        public GetTwilioClientOptions(IOptionsMonitor<InfraSettings> settingsAccessor)
            : base(settingsAccessor.CurrentValue.TwilioClientSettings)
        {
        }
    }
}
