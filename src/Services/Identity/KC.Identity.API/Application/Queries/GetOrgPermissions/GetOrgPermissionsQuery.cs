using System.Collections.Generic;
using MediatR;

namespace KC.Identity.API.Application
{
    public readonly record struct GetOrgPermissionsQuery(int OrgId) : IRequest<List<PermissionDto>>;
}
