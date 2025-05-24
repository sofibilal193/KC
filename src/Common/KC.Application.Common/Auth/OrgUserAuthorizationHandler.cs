using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using KC.Application.Common.Identity;
using KC.Domain.Common.Exceptions;
using KC.Domain.Common.ApiServices;

namespace KC.Application.Common.Auth
{
    public class OrgUserAuthorizationHandler : AuthorizationHandler<OrgUserRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApiService _apiService;

        public OrgUserAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IApiService apiService)
        {
            _httpContextAccessor = httpContextAccessor;
            _apiService = apiService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OrgUserRequirement requirement)
        {
            if (_httpContextAccessor.HttpContext is not null)
            {
                var routeValues = _httpContextAccessor.HttpContext.Request.RouteValues;
                if (routeValues.TryGetValue("orgId", out var orgIdParam))
                {
                    if (int.TryParse(orgIdParam?.ToString(), out var orgId))
                    {
                        if (context.User.IsSuperAdmin())
                        {
                            context.Succeed(requirement);
                        }
                        else if (context.User.IsMemberOfOrg(orgId))
                        {
                            context.Succeed(requirement);
                        }
                        else if (!_httpContextAccessor.HttpContext.IsApiKeyAuth()
                            && context.User.ContainsOrgIdClaim() && context.User.ContainsUserIdClaim())
                        {
                            var url = $"auth/orgAccess?orgId={orgId}";
                            var hasAccess = await _apiService.GetAsync<bool>(ApiServiceTypes.Identity, url);
                            if (!hasAccess)
                            {
                                context.Fail();
                            }
                        }
                    }
                    else
                    {
                        throw new NotFoundException();
                    }
                }
                context.Succeed(requirement);
            }
        }
    }
}
