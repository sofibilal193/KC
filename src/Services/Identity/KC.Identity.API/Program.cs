using KC.Infrastructure.Common.AppConfig;
using KC.Identity.API;

namespace KC.Identity.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            args.CreateHostBuilder<Startup>()
                .AddAppConfig()
                .Build()
                .Run();
        }
    }
}