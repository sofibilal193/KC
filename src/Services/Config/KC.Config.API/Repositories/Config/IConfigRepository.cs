using KC.Config.API.Entities;
using KC.Persistence.Common.Repositories;

namespace KC.Config.API.Repositories
{
    public interface IConfigRepository : ISqlRepository<ConfigItem>
    {
    }
}
