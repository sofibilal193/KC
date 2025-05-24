using KC.Identity.API.Entities;
using KC.Identity.API.Persistence;
using KC.Persistence.Common.Repositories;

namespace KC.Identity.API.Repositories
{
    public class PermissionRepository : SqlRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(IdentityDbContext context) : base(context)
        {
        }
    }
}
