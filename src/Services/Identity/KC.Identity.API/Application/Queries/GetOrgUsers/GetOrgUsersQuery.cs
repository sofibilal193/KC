using System.Collections.Generic;
using MediatR;

namespace KC.Identity.API.Application
{
    /// <summary>
    /// Get request for getting a list of users for an Organization.
    /// </summary>
    /// <param name="OrgId">ID of Organization.</param>
    public readonly record struct GetOrgUsersQuery(int OrgId)
        : IRequest<IList<OrgUserDto>>;
}
