using System.Threading;
using System.Threading.Tasks;

namespace KC.Infrastructure.Common.Messaging
{
    public interface IBitlyUrlHttpClient
    {
        Task<string> GetShortUrlAsync(string longUrl,
            CancellationToken cancellationToken = default);
    }
}
