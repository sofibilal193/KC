

using KC.Identity.API.Persistence;
using KC.Infrastructure.Common.AppConfig;
using KC.Persistence.Common;
using KC.Persistence.Common.Events;

namespace KC.Mono.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            args.CreateHostBuilder<Startup>()
                .AddAppConfig()
                .Build()
                .MigrateDbContext<EventSqlDbContext>((_, _) => { })
                .MigrateDbContext<IdentityDbContext>((_, _) => { })
                .Run();
        }
    }

}