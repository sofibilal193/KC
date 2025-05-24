using System.Collections.Generic;
using MediatR;
using KC.Domain.Common.Identity;
using KC.Identity.API.Application;

namespace KC.Identity.API.Application
{
    public readonly record struct GetUserOrgImpersonationQuery(int OrgId)
        : IRequest<ImpersonateUserResponse>;
}
