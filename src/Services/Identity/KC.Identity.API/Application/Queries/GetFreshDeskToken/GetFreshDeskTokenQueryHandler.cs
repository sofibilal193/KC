using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using KC.Domain.Common.Identity;
using KC.Utils.Common;
using KC.Domain.Common;

namespace KC.Identity.API.Application.Queries
{
    public class GetFreshDeskTokenQueryHandler : IRequestHandler<GetFreshDeskTokenQuery, string?>
    {
        private readonly ICurrentUser _user;
        private readonly FreshDeskSettings _settings;
        private readonly IDateTime _dt;

        public GetFreshDeskTokenQueryHandler(ICurrentUser user, IOptionsMonitor<FreshDeskSettings> settings,
            IDateTime dt)
        {
            _user = user;
            _settings = settings.CurrentValue;
            _dt = dt;
        }

        public Task<string?> Handle(GetFreshDeskTokenQuery request, CancellationToken cancellationToken)
        {
            string? token = !string.IsNullOrEmpty(_settings.SharedSecretKey)
                ? JwtUtil.GenerateToken(_user.UserId, _user.FullName, _user.Email,
                _settings.SharedSecretKey, "https://onedealerlane.com", "FreshDesk", _dt.Now.AddHours(2))
                : null;
            return Task.FromResult(token);
        }
    }
}
