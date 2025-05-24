using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MediatR;
using KC.Application.Common.Identity;
using KC.Domain.Common;
using KC.Domain.Common.Identity;
using KC.Identity.API.Notifications;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetUserOrgImpersonationQueryHandler : IRequestHandler<GetUserOrgImpersonationQuery, ImpersonateUserResponse?>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMediator _mediator;
        private readonly ICurrentUser _currentUser;
        private readonly IDateTime _dt;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserOrgImpersonationQueryHandler(IIdentityUnitOfWork data, IMediator mediator,
            ICurrentUser user, IDateTime dt, IHttpContextAccessor httpContextAccessor)
        {
            _data = data;
            _mediator = mediator;
            _currentUser = user;
            _dt = dt;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ImpersonateUserResponse?> Handle(GetUserOrgImpersonationQuery request,
            CancellationToken cancellationToken)
        {
            bool isAllowed = false;
            if (_httpContextAccessor.HttpContext is not null)
            {
                if (_httpContextAccessor.HttpContext.User.IsSuperAdmin())
                {
                    isAllowed = true;
                }
                else if (_httpContextAccessor.HttpContext.User.IsMemberOfOrg(request.OrgId))
                {
                    isAllowed = true;
                }
            }

            if (isAllowed)
            {
                var roles = await _data.Roles.GetRolePermissionsAsync("Administrator", cancellationToken);
                if (roles is null)
                {
                    return null;
                }

                var log = new IdentityEventLogNotification("ImpersonateUser", _dt.Now,
                _currentUser.Source, "", _currentUser.UserId, request.OrgId, roles.Id);
                await _mediator.Publish(log, cancellationToken);

                return new ImpersonateUserResponse
                {
                    Title = "Admin",
                    Role = "Administrator",
                    Permissions = roles.Permissions.Select(p => p.Name).ToList()
                };
            }
            return null;
        }
    }
}
