using System.Collections.Generic;
using MediatR;

namespace KC.Identity.API.Application
{
    /// <summary>
    /// Query to retrieve a list of roles for the application
    /// </summary>
    public readonly record struct GetRolesQuery(int? OrgId) : IRequest<IList<RoleDto>>;
}
