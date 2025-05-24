using KC.Config.API.Entities;
using KC.Config.API.Persistence;
using KC.Persistence.Common.Repositories;

namespace KC.Config.API.Repositories
{
    public class UserConfigValueRepository : SqlRepository<UserConfigFieldValue>, IUserConfigValueRepository
    {
        public UserConfigValueRepository(ConfigDbContext context) : base(context)
        {
        }
    }
}
