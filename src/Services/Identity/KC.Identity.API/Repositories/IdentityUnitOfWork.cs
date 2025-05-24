using System;
using System.Threading;
using System.Threading.Tasks;
using KC.Identity.API.Persistence;

namespace KC.Identity.API.Repositories
{
    public class IdentityUnitOfWork : IIdentityUnitOfWork
    {
        private readonly IdentityDbContext _context;
        private bool _disposedValue;

        public IOrgRepository Orgs { get; }

        public IUserRepository Users { get; }

        public IRoleRepository Roles { get; }

        public IPermissionRepository Permissions { get; }

        public IdentityUnitOfWork(IdentityDbContext context,
            IOrgRepository orgs,
            IUserRepository users,
            IRoleRepository roles,
            IPermissionRepository permissions)
        {
            _context = context;

            // repositories
            Orgs = orgs;
            Users = users;
            Roles = roles;
            Permissions = permissions;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveEntitiesAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
