using System;
using System.Linq;
using MediatR;

namespace KC.Application.Common.Cacheing
{
    // Using C# 8.0 to provide a default interface implementation.
    // Optionally, could move this to an AbstractCachingPolicy like AbstractValidator.
    // https://anderly.com/2019/12/12/cross-cutting-concerns-with-mediatr-pipeline-behaviors/
    public interface ICachePolicy<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        DateTimeOffset? AbsoluteExpiration(TRequest request, TResponse response) => null;
        TimeSpan? AbsoluteExpirationRelativeToNow(TRequest request, TResponse response) => null;
        TimeSpan? SlidingExpiration(TRequest request, TResponse response) => TimeSpan.FromMinutes(15);

        string GetCacheKey(TRequest request)
        {
            var r = new { request };
            var props = r.request.GetType().GetProperties().Select(pi => $"{pi.Name}:{pi.GetValue(r.request, null)}");
            return $"{typeof(TRequest).FullName}{{{String.Join(",", props)}}}";
        }
    }
}
