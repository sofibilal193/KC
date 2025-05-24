using KC.Identity.API.Entities;
using KC.Persistence.Common.Repositories;

namespace KC.Identity.API.Repositories
{
    public interface IPermissionRepository : ISqlRepository<Permission>
    {
    }
}
