using System.Threading;
using System.Threading.Tasks;
using KC.Identity.API.Entities;
using KC.Persistence.Common.Repositories;

namespace KC.Identity.API.Repositories
{
    public interface IRoleRepository : ISqlRepository<Role>
    {
        public Task<Role?> GetRolePermissionsAsync(string name, CancellationToken cancellationToken = default);
    }
}
