using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using KC.Application.Common.Cacheing;
using KC.Domain.Common.Identity;
using KC.Identity.API.Extensions;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetCurrentUserOrgIdsQueryHandler : IRequestHandler<GetCurrentUserOrgIdsQuery, IEnumerable<int>>
    {
        private readonly ICache _cache;
        private readonly IIdentityUnitOfWork _data;
        private readonly ICurrentUser _user;

        public GetCurrentUserOrgIdsQueryHandler(IIdentityUnitOfWork data, ICurrentUser user, ICache cache)
        {
            _data = data;
            _user = user;
            _cache = cache;
        }

        public async Task<IEnumerable<int>> Handle(GetCurrentUserOrgIdsQuery request, CancellationToken cancellationToken)
        {
            var orgIds = new List<int>();
            var groups = await _cache.GetOrgGroupsAsync();
            if (groups is null)
            {
                groups = await _data.Orgs.GetOrgGroupsAsync(cancellationToken);
                await _cache.SetOrgGroupsAsync(groups);
            }

            foreach (var orgId in _user.OrgIds)
            {
                var childOrgIds = groups.Where(g => g.ParentOrgId == orgId).Select(g => g.OrgId).ToList();
                if (childOrgIds.Count > 0)
                {
                    if (request.IncludeGroups)
                    {
                        orgIds.Add(orgId);
                    }
                    while (childOrgIds.Count > 0)
                    {
                        var groupOrgIds = childOrgIds.Where(id => groups.Exists(g => g.ParentOrgId == id));
                        orgIds.AddRange(request.IncludeGroups ? childOrgIds : childOrgIds.Except(groupOrgIds));
                        childOrgIds = groups.Where(g => groupOrgIds.Contains(g.ParentOrgId)).Select(g => g.OrgId).ToList();
                    }
                }
                else
                {
                    orgIds.Add(orgId);
                }
            }

            return orgIds.Distinct().Order();
        }
    }
}
