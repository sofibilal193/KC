using System;
using System.Threading;
using System.Threading.Tasks;

namespace KC.Application.Common.Messaging
{
    public interface IUrlProvider : IDisposable
    {
        Task<string> ShortenUrlAsync(string longUrl,
            CancellationToken cancellationToken = default);
    }
}
