using KC.Config.API.Entities;
using KC.Config.API.Persistence;
using KC.Persistence.Common.Repositories;

namespace KC.Config.API.Repositories
{
    public class ConfigRepository : SqlRepository<ConfigItem>, IConfigRepository
    {
        public ConfigRepository(ConfigDbContext context) : base(context)
        {
        }
    }
}
