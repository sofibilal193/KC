using KC.Config.API.Persistence;
using KC.Infrastructure.Common.AppConfig;
using KC.Persistence.Common;
using KC.Persistence.Common.Events;

namespace KC.Config.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            args.CreateHostBuilder<Startup>()
                .AddAppConfig()
                .Build()
                .MigrateDbContext<EventSqlDbContext>((_, _) => { })
                .MigrateDbContext<ConfigDbContext>((_, _) => { })
                .Run();
        }
    }
}