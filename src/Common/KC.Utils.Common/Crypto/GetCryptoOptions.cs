using System;
using Microsoft.Extensions.Options;

namespace KC.Utils.Common.Crypto
{
    public record GetCryptoOptions : CryptoOptions
    {
        public GetCryptoOptions(IOptionsMonitor<CryptoSettings> settingsAccessor, ISecretRepository secretRepository)
            : base(secretRepository.UnWrapKeyAsync(settingsAccessor.CurrentValue.WrappedEncryptionKeyBase64Encoded,
                settingsAccessor.CurrentValue.EncryptionWrapKeyName).GetAwaiter().GetResult() ?? Array.Empty<byte>())
        {
            if (string.IsNullOrEmpty(settingsAccessor.CurrentValue.WrappedEncryptionKeyBase64Encoded))
                throw new ArgumentNullException(nameof(settingsAccessor), "CryptoSettings.WrappedEncryptionKeyBase64Encoded is null/empty");
            if (string.IsNullOrEmpty(settingsAccessor.CurrentValue.EncryptionWrapKeyName))
                throw new ArgumentNullException(nameof(settingsAccessor), "CryptoSettings.EncryptionWrapKeyName is null/empty");
        }
    }
}
