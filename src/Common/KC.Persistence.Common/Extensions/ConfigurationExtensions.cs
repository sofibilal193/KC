using Microsoft.Extensions.Configuration;
using KC.Persistence.Common.Cosmos;
using KC.Persistence.Common.Settings;

namespace KC.Persistence.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static SqlServerSettings GetSqlServerSettings(this IConfiguration configuration, string applicationName)
        {
            return configuration.GetSection(applicationName).GetSection("PersistenceSettings")
                .GetSection("SqlServer").Get<SqlServerSettings>() ?? new();
        }

        public static CosmosDbRepositoryOptions GetCosmosDbSettings(this IConfiguration configuration, string applicationName)
        {
            return configuration.GetSection(applicationName).GetSection("PersistenceSettings")
                .GetSection("CosmosDb").Get<CosmosDbRepositoryOptions>() ?? new();
        }
    }
}
