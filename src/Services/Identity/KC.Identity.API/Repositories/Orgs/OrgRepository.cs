using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KC.Domain.Common.Extensions;
using KC.Domain.Common.Identity;
using KC.Domain.Common.Types;
using KC.Identity.API.Entities;
using KC.Identity.API.Persistence;
using KC.Persistence.Common.Extensions;
using KC.Persistence.Common.Repositories;

namespace KC.Identity.API.Repositories
{
    [ExcludeFromCodeCoverage]
    public class OrgRepository : SqlRepository<Org>, IOrgRepository
    {
        private readonly IdentityDbContext _context;

        public OrgRepository(IdentityDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<int>> GetSuperAdminOrgIdsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orgs.Where(o => o.Type == OrgType.SuperAdmin && !o.IsDisabled)
                .Select(o => o.Id).ToListAsync(cancellationToken);
        }

        public async Task<List<OrgGroup>> GetOrgGroupsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orgs.AsNoTracking().Include(o => o.Groups)
                .Where(o => !o.IsDisabled).SelectMany(o => o.Groups).ToListAsync(cancellationToken);
        }

        public async Task<PagedList<Org>?> GetUserOrgsAsync(OrgType userOrgType, List<int> userOrgIds,
            int? parentOrgId, int page, int pageSize, string? search, string sortExpression)
        {
            var query = _context.Orgs.Include(o => o.Groups).AsNoTracking()
                .Where(o => !o.IsDisabled && (o.Type == OrgType.Group || o.Type == OrgType.Dealer));

            if (parentOrgId.HasValue)
            {
                query = query.Where(o => o.Groups.Select(g => g.ParentOrgId).Contains(parentOrgId.Value));
            }
            else if (userOrgType == OrgType.SuperAdmin)
            {
                query = query.Where(o => o.Groups.Count == 0);
            }
            else if (userOrgType == OrgType.Group)
            {
                query = query.Where(o => o.Groups.Select(g => g.ParentOrgId).Any(id => userOrgIds.Contains(id)));
            }
            else
            {
                query = _context.Orgs.Where(d => userOrgIds.Contains(d.Id));
            }

            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out var id))
                {
                    query = query.Where(d => d.Id == id);
                }
                else
                {
                    query = query.Where(d => (d.Name ?? "").StartsWith(search) || (d.Name ?? "").Contains(" " + search));
                }
            }

            return await query.OrderBy(sortExpression).ToPagedListAsync(page, pageSize);
        }

        public async Task<OrgType?> GetOrgTypeAsync(int orgId, CancellationToken cancellationToken = default)
        {
            return await _context.Orgs.AsNoTracking().Where(o => o.Id == orgId)
                .Select(o => o.Type).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> GetOrgCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orgs.AsNoTracking().CountAsync(o => !o.IsDisabled &&
                o.Type != OrgType.SuperAdmin && o.Type != OrgType.Group, cancellationToken);
        }
    }
}
