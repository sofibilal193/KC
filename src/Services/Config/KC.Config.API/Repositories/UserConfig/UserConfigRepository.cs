using KC.Config.API.Entities;
using KC.Config.API.Persistence;
using KC.Persistence.Common.Repositories;

namespace KC.Config.API.Repositories
{
    public class UserConfigRepository : SqlRepository<UserConfigField>, IUserConfigRepository
    {
        public UserConfigRepository(ConfigDbContext context) : base(context)
        {
        }
    }
}
