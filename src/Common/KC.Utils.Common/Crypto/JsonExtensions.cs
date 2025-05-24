using System.Text.Json;
using KC.Domain.Common.Extensions;

namespace KC.Utils.Common.Crypto
{
    public static class JsonExtensions
    {
        public static T? DecryptPropertyValue<T>(this JsonElement element,
            string propertyName, ICryptoProvider cryptoProvider, byte[]? key = null)
        {
            var stringValue = element.GetPropertyValue<string>(propertyName);
            if (stringValue is not null)
            {
                var decryptedString = cryptoProvider.DecryptString(stringValue, key);
                if (decryptedString is not null && typeof(T).TryParse(decryptedString, out object? value))
                {
                    return (T?)value;
                }
            }
            return default;
        }

        public static T? DecryptPropertyValue<T>(this JsonDocument document,
            string propertyName, ICryptoProvider cryptoProvider, byte[]? key = null)
        {
            return document.RootElement.DecryptPropertyValue<T>(propertyName, cryptoProvider, key);
        }
    }
}
