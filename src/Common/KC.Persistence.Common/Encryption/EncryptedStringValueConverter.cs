using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using KC.Utils.Common.Crypto;

namespace KC.Persistence.Common
{
    internal sealed class EncryptedStringValueConverter : ValueConverter<string, string>
    {
        public EncryptedStringValueConverter(ICryptoProvider cryptoProvider, byte[]? encryptionKey = null, ConverterMappingHints? mappingHints = default)
            : base(x => cryptoProvider != null ? cryptoProvider.EncryptString(x, encryptionKey) ?? x : x,
                x => cryptoProvider != null ? cryptoProvider.DecryptString(x, encryptionKey) ?? x : x, mappingHints)
        {
        }
    }
}
