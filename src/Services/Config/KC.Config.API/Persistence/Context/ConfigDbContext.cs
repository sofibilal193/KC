using Microsoft.EntityFrameworkCore;
using MediatR;
using KC.Config.API.Entities;
using KC.Domain.Common;
using KC.Domain.Common.Identity;
using KC.Persistence.Common;
using KC.Utils.Common.Crypto;

namespace KC.Config.API.Persistence
{
    public class ConfigDbContext : BaseSqlDbContext<ConfigDbContext>
    {
        public override string DefaultSchema => Constants.SchemaName;

        public DbSet<ConfigItem> ConfigItems => Set<ConfigItem>();

        public DbSet<OrgConfigField> OrgConfigFields => Set<OrgConfigField>();

        public DbSet<OrgConfigFieldValue> OrgConfigFieldValues => Set<OrgConfigFieldValue>();

        public DbSet<UserConfigField> UserConfigFields => Set<UserConfigField>();

        public DbSet<UserConfigFieldValue> UserConfigFieldValues => Set<UserConfigFieldValue>();

        public ConfigDbContext(DbContextOptions<ConfigDbContext> options, ILogger<ConfigDbContext> logger, ICryptoProvider cryptoProvider, ICurrentUser currentUser, IDateTime dateTime, IMediator mediator)
            : base(options, logger, cryptoProvider, currentUser, dateTime, mediator)
        { }
    }
}
