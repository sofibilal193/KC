using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KC.Domain.Common;
using KC.Utils.Common.Crypto;
using KC.Domain.Common.Identity;

namespace KC.Persistence.Common.Events
{
    public class EventSqlDbContextFactory : DesignTimeDbContextFactoryBase<EventSqlDbContext>
    {
        // public override string ConnectionStringName => "EventDbConnection";

        public override EventSqlDbContext CreateNewInstance(DbContextOptions<EventSqlDbContext> options, ILogger<EventSqlDbContext> logger, IMediator mediator)
        {
            CryptoOptions? cryptoOptions = null;
            return new EventSqlDbContext(options, logger, new CryptoProvider(cryptoOptions), new NoCurrentUser(), new UtcDateTime(), mediator);
        }
    }
}
