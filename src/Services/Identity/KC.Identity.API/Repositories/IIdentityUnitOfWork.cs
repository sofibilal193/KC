using KC.Domain.Common.Repositories;

namespace KC.Identity.API.Repositories
{
    public interface IIdentityUnitOfWork : IUnitOfWork
    {
        IOrgRepository Orgs { get; }

        IUserRepository Users { get; }

        IRoleRepository Roles { get; }

        IPermissionRepository Permissions { get; }
    }
}
