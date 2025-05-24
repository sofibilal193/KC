using System;
using System.Threading;
using System.Threading.Tasks;
using KC.Application.Common.Messaging;

namespace KC.Infrastructure.Common.Messaging
{
    /// <summary>
    /// Url provider class that implements IUrlProvider
    /// </summary>
    public class BitlyUrlProvider : IUrlProvider
    {
        #region Private Properties

        private readonly IBitlyUrlHttpClient _client;

        #endregion

        #region Constructors

        public BitlyUrlProvider(IBitlyUrlHttpClient client)
        {
            _client = client;
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

        #region IUrlProvider Methods

        public async Task<string> ShortenUrlAsync(string longUrl,
            CancellationToken cancellationToken = default)
        {
            return await _client.GetShortUrlAsync(longUrl, cancellationToken);
        }

        #endregion

    }
}
