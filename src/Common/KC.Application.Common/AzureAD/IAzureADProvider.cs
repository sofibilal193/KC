using System;
using System.Threading;
using System.Threading.Tasks;
using KC.Domain.Common.AzureAD;

namespace KC.Application.Common.AzureAD
{
    public interface IAzureADProvider : IDisposable
    {
        Task<UpdateUserResponse> UpdateUserAsync(AzureADUser user,
            CancellationToken cancellationToken = default);
    }

    public enum UpdateUserResponse : byte
    {
        Success,
        Failed,
        Duplicate,
        Notfound
    }
}
