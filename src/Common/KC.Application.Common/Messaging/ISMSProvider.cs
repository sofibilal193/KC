using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KC.Domain.Common.Messaging;

namespace KC.Application.Common.Messaging
{
    public interface ISmsProvider : IDisposable
    {
        Task SendAsync(IList<SmsMessage> messages,
            CancellationToken cancellationToken = default);
    }
}
