using System;
using KC.Application.Common.Cacheing;

namespace KC.Identity.API.Application.Queries
{
    public class GetFreshDeskTokenQueryCachePolicy : ICachePolicy<GetFreshDeskTokenQuery, string?>
    {
        public TimeSpan? SlidingExpiration(GetFreshDeskTokenQuery query, string? dto) => TimeSpan.FromMinutes(30);

        public string GetCacheKey(GetFreshDeskTokenQuery query)
        {
            return $"GetFreshDeskTokenQuery.{query.UserId}";
        }
    }
}
