using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using KC.Domain.Common;
using KC.Domain.Common.Identity;
using KC.Identity.API.Entities;
using KC.Persistence.Common;
using KC.Utils.Common.Crypto;

namespace KC.Identity.API.Persistence
{
    [ExcludeFromCodeCoverage]
    public class IdentityDbContext : BaseSqlDbContext<IdentityDbContext>
    {
        public override string DefaultSchema => "id";

        public DbSet<User> Users => Set<User>();
        public DbSet<Org> Orgs => Set<Org>();
        public DbSet<OrgUser> OrgUsers => Set<OrgUser>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options, ILogger<IdentityDbContext> logger, ICryptoProvider cryptoProvider, ICurrentUser currentUser, IDateTime dateTime, IMediator mediator)
            : base(options, logger, cryptoProvider, currentUser, dateTime, mediator)
        { }
    }
}
