using Microsoft.Extensions.Options;
using KC.Utils.Common;

namespace KC.Infrastructure.Common.Crypto
{
    public record GetSecretRepoOptions : SecretRepoOptions
    {
        public GetSecretRepoOptions(IOptionsMonitor<CryptoSettings> settingsAccessor)
            : base(settingsAccessor.CurrentValue.AzureKeyVaultUri)
        {
        }
    }
}
