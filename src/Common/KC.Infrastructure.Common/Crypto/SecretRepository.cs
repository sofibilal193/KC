using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using KC.Utils.Common.Crypto;

namespace KC.Infrastructure.Common.Crypto
{
    public class SecretRepository : ISecretRepository
    {
        private readonly string _wrapAlgorithm = "RSA-OAEP-256";
        private readonly SecretClient _secretClient;
        private readonly KeyClient _keyClient;

        #region Constructors

        public SecretRepository(SecretClient secretClient, KeyClient keyClient)
        {
            _secretClient = secretClient;
            _keyClient = keyClient;
        }

        #endregion

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // dispose resources
        }

        #endregion

        #region ISecretRepository Methods

        #region Secrets

        public async Task<string> GetSecretValueAsync(string name,
            CancellationToken cancellationToken = default)
        {
            var secret = await _secretClient.GetSecretAsync(name, cancellationToken: cancellationToken);
            return secret?.Value?.Value ?? string.Empty;
        }

        public async Task<string> UpsertSecretAsync(string name, string value,
            CancellationToken cancellationToken = default)
        {
            await _secretClient.SetSecretAsync(name, value, cancellationToken);
            return new Uri(_secretClient.VaultUri, "secrets/" + name).ToString();
        }

        public async Task DeleteSecretAsync(string name, CancellationToken cancellationToken = default)
        {
            var op = await _secretClient.StartDeleteSecretAsync(name, cancellationToken);
            while (!op.HasCompleted)
            {
                await Task.Delay(2000, cancellationToken);
                await op.UpdateStatusAsync(cancellationToken);
            }
            DeletedSecret secret = op.Value;
            await _secretClient.PurgeDeletedSecretAsync(secret.Name, cancellationToken);
        }

        #endregion

        #region Keys

        public async Task<string> WrapKeyAsync(byte[] key, string keyName, CancellationToken cancellationToken = default)
        {
            var value = string.Empty;
            var result = await _keyClient.GetCryptographyClient(keyName)
                .WrapKeyAsync(_wrapAlgorithm, key, cancellationToken);
            if (result?.EncryptedKey != null)
            {
                value = Convert.ToBase64String(result.EncryptedKey);
            }
            return value;
        }

        public async Task<byte[]> UnWrapKeyAsync(string base64EncodedWrappedKey, string keyName, CancellationToken cancellationToken = default)
        {
            var result = await _keyClient.GetCryptographyClient(keyName)
                .UnwrapKeyAsync(_wrapAlgorithm, Convert.FromBase64String(base64EncodedWrappedKey)
                , cancellationToken);
            return result?.Key ?? Array.Empty<byte>();
        }

        #endregion

        #endregion
    }
}
