using System.Collections.Generic;
using MediatR;

namespace KC.Identity.API.Application
{
    public readonly record struct GetOrgRolesQuery(int OrgId) : IRequest<List<OrgRoleDto>>;
}
