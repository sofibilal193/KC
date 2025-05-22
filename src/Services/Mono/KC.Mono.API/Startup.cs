namespace KC.Mono.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }
        private IServiceCollection _services = new ServiceCollection();

        // private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration,
             IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
            // _loggerFactory = LoggerFactory.Create(builder => Environment.AddLogging(builder));
        }

        // Register services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddMonoApi(Configuration, Environment);

            _services = services;

        }

        // Configure middleware
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config)
        {
            // app.UseApplication(env, _services, true);

            app.UseMonoApi(config);
            if (env.IsDevelopment())
            {
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
        }
    }

}