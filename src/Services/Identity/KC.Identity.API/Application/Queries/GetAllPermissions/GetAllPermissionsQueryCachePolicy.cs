using System;
using System.Collections.Generic;
using KC.Application.Common.Cacheing;

namespace KC.Identity.API.Application
{
    // Simply defining a CachePolicy for ICachePolicy<TRequest,TResponse> sets up caching
    // similar to setting up a FluentValidation Validator that inherits from AbstractValidator<TRequest>.
    // This could be in the same file or in a separate file, but doesn't clutter up the "Handler".
    // https://anderly.com/2019/12/12/cross-cutting-concerns-with-mediatr-pipeline-behaviors/
    public class GetAllPermissionsQueryCachePolicy : ICachePolicy<GetAllPermissionsQuery, List<PermissionDto>>
    {
        // Optionally, change defaults
        public TimeSpan? SlidingExpiration(GetAllPermissionsQuery query, List<PermissionDto> dto) => TimeSpan.FromMinutes(60);

        // Optionally, provide a different implementation here. By default the CacheKey will be in the following format:
        //     Query{CustomerNumber:001425}
        public string GetCacheKey(GetAllPermissionsQuery query)
        {
            return "GetAllPermissionsQuery";
        }
    }
}
