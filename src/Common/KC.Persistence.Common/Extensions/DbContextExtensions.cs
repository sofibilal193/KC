using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using KC.Domain.Common;
using KC.Domain.Common.Entities;
using KC.Domain.Common.Identity;
using KC.Domain.Common.ValueObjects;

namespace KC.Persistence.Common
{
    public static class DbContextExtensions
    {
        private const string _procedurePath = @"Cosmos\Procedures";

        internal static void TrackEntityChanges(this DbContext ctx, ICurrentUser currentUser, IDateTime dt)
        {
            if (currentUser.UserId is null)
            {
                return;
            }
            foreach (var entry in ctx.ChangeTracker.Entries().Where(x => x.Entity is BaseEntity))
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                if (entry.Entity is BaseEntity baseEntity && (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                {
                    if (!baseEntity.CreateUserId.HasValue)
                    {
                        ((BaseEntity)entry.Entity).CreateUserId = currentUser.UserId;
                    }
                    if (string.IsNullOrEmpty(baseEntity.CreateUserName))
                    {
                        ((BaseEntity)entry.Entity).CreateUserName = currentUser.FullName;
                    }
                    if (string.IsNullOrEmpty(baseEntity.CreateSource))
                    {
                        ((BaseEntity)entry.Entity).CreateSource = currentUser.Source;
                    }
                    if (!baseEntity.CreateDateTimeUtc.HasValue)
                    {
                        ((BaseEntity)entry.Entity).CreateDateTimeUtc = dt.Now;
                    }

                    ((BaseEntity)entry.Entity).ModifyDateTimeUtc = dt.Now;
                    ((BaseEntity)entry.Entity).ModifyUserId = currentUser.UserId;
                    ((BaseEntity)entry.Entity).ModifyUserName = currentUser.FullName;
                    ((BaseEntity)entry.Entity).ModifySource = currentUser.Source;
                }
            }
            foreach (var entry in ctx.ChangeTracker.Entries<Tag>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreateDateTimeUtc = dt.Now;
                    entry.Entity.CreateUserId = currentUser.UserId;
                    entry.Entity.CreateUserName = currentUser.FullName;
                    entry.Entity.CreateSource = currentUser.Source;
                }
                entry.Entity.ModifyDateTimeUtc = dt.Now;
                entry.Entity.ModifyUserId = currentUser.UserId;
                entry.Entity.ModifyUserName = currentUser.FullName;
                entry.Entity.ModifySource = currentUser.Source;
            }
        }

        public static void RegisterCommonProcedures<TContext>(this BaseDocDbContext<TContext> context)
            where TContext : DbContext
        {
            if (context.IsDocument)
            {
                var client = context.Database.GetCosmosClient();
                var dbResponse = client.CreateDatabaseIfNotExistsAsync(context.DbId).GetAwaiter().GetResult();
                var database = dbResponse.Database;

                FeedIterator<ContainerProperties> iterator = database.GetContainerQueryIterator<ContainerProperties>();
                foreach (var cprop in iterator.ReadNextAsync().GetAwaiter().GetResult())
                {
                    var container = database.GetContainer(cprop.Id);
                    string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                        _procedurePath);
                    if (Directory.Exists(path))
                    {
                        foreach (string filePath in Directory.GetFiles(path))
                        {
                            string fileName = Path.GetFileNameWithoutExtension(filePath);
                            try
                            {
                                container.Scripts.ReplaceStoredProcedureAsync(
                                    new Microsoft.Azure.Cosmos.Scripts.StoredProcedureProperties
                                    {
                                        Id = fileName,
                                        Body = File.ReadAllText(filePath)
                                    }
                                ).GetAwaiter().GetResult();
                            }
                            catch (CosmosException cex)
                            {
                                if (cex.StatusCode == System.Net.HttpStatusCode.NotFound)
                                {
                                    container.Scripts.CreateStoredProcedureAsync(
                                        new Microsoft.Azure.Cosmos.Scripts.StoredProcedureProperties
                                        {
                                            Id = fileName,
                                            Body = File.ReadAllText(filePath)
                                        }
                                    ).GetAwaiter().GetResult();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
