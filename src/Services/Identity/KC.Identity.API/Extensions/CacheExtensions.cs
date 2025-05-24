using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Application.Common.Cacheing;
using KC.Identity.API.Entities;

namespace KC.Identity.API.Extensions
{
    public static class CacheExtensions
    {
        private const string _superAdminOrgIdsKey = "SuperAdminOrgIds";

        private const string _orgGroupsKey = "OrgGroups";

        #region SuperAdminOrgIds

        public static async Task<List<int>?> GetSuperAdminOrgIdsAsync(this ICache cache)
        {
            return await cache.GetAsync<List<int>>(_superAdminOrgIdsKey);
        }

        public static async Task SetSuperAdminOrgIdsAsync(this ICache cache, List<int> orgIds)
        {
            await cache.SetAsync(_superAdminOrgIdsKey, orgIds);
        }

        public static async Task ClearSuperAdminOrgIdsAsync(this ICache cache)
        {
            await cache.RemoveAsync(_superAdminOrgIdsKey);
        }

        #endregion

        #region OrgGroups

        public static async Task<List<OrgGroup>?> GetOrgGroupsAsync(this ICache cache)
        {
            return await cache.GetAsync<List<OrgGroup>>(_orgGroupsKey);
        }

        public static async Task SetOrgGroupsAsync(this ICache cache, List<OrgGroup> groups)
        {
            await cache.SetAsync(_orgGroupsKey, groups);
        }

        public static async Task ClearOrgGroupsAsync(this ICache cache)
        {
            await cache.RemoveAsync(_orgGroupsKey);
        }

        #endregion
    }
}
