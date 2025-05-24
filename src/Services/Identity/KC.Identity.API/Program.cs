using KC.Infrastructure.Common.AppConfig;
using ODL.Identity.API;

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