using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KC.Domain.Common.Identity;
using KC.Domain.Common.Types;
using KC.Identity.API.Application;
using KC.Identity.API.Entities;
using KC.Persistence.Common.Repositories;

namespace KC.Identity.API.Repositories
{
    public interface IUserRepository : ISqlRepository<User>
    {
        Task<PagedList<User>> GetOrgUsersAsync(OrgType orgType, List<int> orgIds,
            int page, int pageSize, string? search, string sortExpression, CancellationToken cancellationToken = default);

        Task<int> GetOrgUsersCountAsync(OrgType orgType, List<int> orgIds, CancellationToken cancellationToken = default);

        Task<List<PermissionUserDto>> GetAllUsersByPermissionsAsync(int orgId, List<string> permissions, CancellationToken cancellationToken = default);

        public Task<List<SearchUserDto>> SearchAsync(string search, CancellationToken cancellationToken = default);
    }
}
