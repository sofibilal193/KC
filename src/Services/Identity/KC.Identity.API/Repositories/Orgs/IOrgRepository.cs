using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KC.Domain.Common.Identity;
using KC.Domain.Common.Types;
using KC.Identity.API.Entities;
using KC.Persistence.Common.Repositories;

namespace KC.Identity.API.Repositories
{
    public interface IOrgRepository : ISqlRepository<Org>
    {
        Task<List<int>> GetSuperAdminOrgIdsAsync(CancellationToken cancellationToken = default);

        Task<List<OrgGroup>> GetOrgGroupsAsync(CancellationToken cancellationToken = default);

        Task<PagedList<Org>?> GetUserOrgsAsync(OrgType userOrgType, List<int> userOrgIds, int? parentOrgId, int page,
            int pageSize, string? search, string sortExpression);

        Task<OrgType?> GetOrgTypeAsync(int orgId, CancellationToken cancellationToken = default);

        Task<int> GetOrgCountAsync(CancellationToken cancellationToken = default);
    }
}
