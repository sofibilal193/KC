using System;
using System.Collections.Generic;
using KC.Application.Common.Cacheing;

namespace KC.Identity.API.Application
{
    public class GetAllUsersByPermissionsQueryCachePolicy : ICachePolicy<GetAllUsersByPermissionsQuery, List<PermissionUserDto>>
    {
        public TimeSpan? SlidingExpiration(GetAllUsersByPermissionsQuery query) => TimeSpan.FromMinutes(60);

        public string GetCacheKey(GetAllUsersByPermissionsQuery query)
        {
            return $"GetAllUsersByPermissionsQuery.{query.OrgId}.{String.Join(",", query.Permissions)}";
        }
    }
}
