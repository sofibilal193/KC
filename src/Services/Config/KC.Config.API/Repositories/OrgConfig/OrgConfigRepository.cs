using KC.Config.API.Entities;
using KC.Config.API.Persistence;
using KC.Persistence.Common.Repositories;

namespace KC.Config.API.Repositories
{
    public class OrgConfigRepository : SqlRepository<OrgConfigField>, IOrgConfigRepository
    {
        public OrgConfigRepository(ConfigDbContext context) : base(context)
        {
        }
    }
}
