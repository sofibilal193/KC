using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KC.Domain.Common.Messaging;

namespace KC.Application.Common.Messaging
{
    public interface IEmailProvider : IDisposable
    {
        Task<bool> SendAsync(IList<EmailMessage> messages,
            CancellationToken cancellationToken = default);
    }
}
