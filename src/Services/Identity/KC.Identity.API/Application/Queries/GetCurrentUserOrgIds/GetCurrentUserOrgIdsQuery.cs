using System.Collections.Generic;
using MediatR;

namespace KC.Identity.API.Application
{
    /// <summary>
    /// Gets a list of Ids of all orgs that the current user has access to.
    /// </summary>
    /// <param name="IncludeGroups">Whether to include Ids or org groups.</param>
    public record GetCurrentUserOrgIdsQuery(bool IncludeGroups) : IRequest<IEnumerable<int>>;
}
