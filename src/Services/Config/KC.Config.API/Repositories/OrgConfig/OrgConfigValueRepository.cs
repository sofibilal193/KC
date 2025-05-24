using KC.Config.API.Entities;
using KC.Config.API.Persistence;
using KC.Persistence.Common.Repositories;

namespace KC.Config.API.Repositories
{
    public class OrgConfigValueRepository : SqlRepository<OrgConfigFieldValue>, IOrgConfigValueRepository
    {
        public OrgConfigValueRepository(ConfigDbContext context) : base(context)
        {
        }
    }
}
