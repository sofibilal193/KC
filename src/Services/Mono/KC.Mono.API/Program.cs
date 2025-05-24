using KC.Infrastructure.Common.AppConfig;

namespace KC.Mono.API
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