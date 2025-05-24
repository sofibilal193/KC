using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MediatR;

namespace KC.Persistence.Common
{
    [ExcludeFromCodeCoverage]
    public abstract class DesignTimeDbContextFactoryBase<TContext> :
        IDesignTimeDbContextFactory<TContext> where TContext : BaseDbContext<TContext>
    {
        public virtual string ConnectionStringName => "DefaultConnection";
        public virtual string AspNetCoreEnvironment => "ASPNETCORE_ENVIRONMENT";

        // public abstract TContext CreateNewInstance(DbContextOptions<TContext> options, ILogger<TContext> logger, IMediator mediator);
        public abstract TContext CreateNewInstance(DbContextOptions<TContext> options, ILogger<TContext> logger, IMediator mediator);

        public TContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory(); // + string.Format("{0}..{0}Identity.API", Path.DirectorySeparatorChar);
            var configuration = new ConfigurationBuilder()
                // .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.local.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable(AspNetCoreEnvironment)}.json", optional: true)
                .AddEnvironmentVariables()
                .AddUserSecrets(GetType().Assembly)
                .Build();

            var serviceProvider = new ServiceCollection()
            .AddLogging(options =>
            {
                options.AddConfiguration(configuration.GetSection("Logging"));
                options.AddConsole();
                options.AddDebug();
            })
            .BuildServiceProvider();
            var factory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = factory.CreateLogger<TContext>();
            logger.LogInformation("Creating DbContextSqlServer...");
            logger.LogInformation("Current Directory: {BasePath}", basePath);
            logger.LogInformation("Environment: {Env}", Environment.GetEnvironmentVariable(AspNetCoreEnvironment));
            logger.LogInformation("Assembly: {Assembly}", GetType().Assembly);

            var connectionString = configuration.GetConnectionString(ConnectionStringName);
            if (!string.IsNullOrEmpty(connectionString))
            {
                var optionsBuilder = new DbContextOptionsBuilder<TContext>();
                optionsBuilder.UseSqlServer(connectionString);
                optionsBuilder.AddInterceptors(new AzureAdAuthDbConnectionInterceptor());
                return CreateNewInstance(optionsBuilder.Options, logger, new NoMediator());
            }

            logger.LogCritical("Connection string '{ConnectionStringName}' is null or empty.", ConnectionStringName);
            throw new ArgumentException($"Connection string '{ConnectionStringName}' is null or empty.");
        }
    }

    public class NoMediator : IMediator
    {
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            return Task.CompletedTask;
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
#pragma warning disable CS8604 // Possible null reference argument for parameter
            return Task.FromResult<TResponse>(default);
#pragma warning restore CS8604 // Possible null reference argument for parameter
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(default(object));
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            throw new NotImplementedException();
        }
    }
}
