using Microsoft.Extensions.Options;

namespace KC.Infrastructure.Common.Cacheing
{
    public record GetRedisCacheOptions : RedisCacheOptions
    {
        public GetRedisCacheOptions(IOptionsMonitor<InfraSettings> settingsAccessor)
            : base(settingsAccessor.CurrentValue.RedisCacheSettings)
        {
        }
    }
}
