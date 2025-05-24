using Microsoft.AspNetCore.SignalR;
using KC.Application.Common.Identity;

namespace KC.Infrastructure.Common.SignalR
{
    /// <summary>
    /// Gets the userId of a user from a hub connection.
    /// </summary>
    public class HubUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User.GetUserId()?.ToString();
        }
    }
}
