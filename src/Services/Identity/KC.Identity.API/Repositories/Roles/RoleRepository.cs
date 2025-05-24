using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KC.Identity.API.Entities;
using KC.Identity.API.Persistence;
using KC.Persistence.Common.Repositories;

namespace KC.Identity.API.Repositories
{
    public class RoleRepository : SqlRepository<Role>, IRoleRepository
    {
        private readonly IdentityDbContext _context;
        public RoleRepository(IdentityDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Role?> GetRolePermissionsAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Roles.AsNoTracking().Where(i => i.Name == name && !i.IsDisabled)
                .Include("Permissions").FirstOrDefaultAsync(cancellationToken);
        }
    }
}
