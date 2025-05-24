using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KC.Domain.Common;
using KC.Utils.Common.Crypto;
using KC.Domain.Common.Identity;
using KC.Persistence.Common;

namespace KC.Identity.API.Persistence
{
    public class IdentityDbContextFactory : DesignTimeDbContextFactoryBase<IdentityDbContext>
    {
        // public override string ConnectionStringName => "IdentityDbConnection";

        public override IdentityDbContext CreateNewInstance(DbContextOptions<IdentityDbContext> options, ILogger<IdentityDbContext> logger, IMediator mediator)
        {
            CryptoOptions? cryptoOptions = null;
            return new IdentityDbContext(options, logger, new CryptoProvider(cryptoOptions), new NoCurrentUser(), new UtcDateTime(), mediator);
        }
    }
}
