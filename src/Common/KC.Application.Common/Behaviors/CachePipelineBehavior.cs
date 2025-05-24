using KC.Application.Common.Cacheing;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KC.Application.Common.Behaviors
{
    /// <summary>
    /// MediatR Caching Pipeline Behavior
    /// https://anderly.com/2019/12/12/cross-cutting-concerns-with-mediatr-pipeline-behaviors/
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class CachePipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<ICachePolicy<TRequest, TResponse>> _cachePolicies;

        // ICache is a helper wrapper over MemoryCache that adds some read-through cache methods, etc.
        private readonly ICache _cache;
        private readonly ILogger<CachePipelineBehavior<TRequest, TResponse>> _logger;

        public CachePipelineBehavior(ICache cache, ILogger<CachePipelineBehavior<TRequest, TResponse>> logger,
            IEnumerable<ICachePolicy<TRequest, TResponse>> cachePolicies)
        {
            _cache = cache;
            _logger = logger;
            _cachePolicies = cachePolicies;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cachePolicy = _cachePolicies.FirstOrDefault();
            if (cachePolicy == null)
            {
                // No cache policy found, so just continue through the pipeline
                return await next();
            }
            var cacheKey = cachePolicy.GetCacheKey(request);
            TResponse? cachedResponse = await _cache.GetAsync<TResponse>(cacheKey);
            if (cachedResponse != null)
            {
                _logger.LogInformation("Response retrieved {otype} from cache. CacheKey: {key}", typeof(TRequest).FullName, cacheKey);
                return cachedResponse;
            }

            var response = await next();
            if (response != null)
            {
                _logger.LogInformation("Caching response for {otype} with cache key: {key}", typeof(TRequest).FullName, cacheKey);

                var expiration = cachePolicy.SlidingExpiration(request, response);
                if (expiration.HasValue)
                {
                    await _cache.SetSlidingAsync(cacheKey, response, expiration.Value);
                    return response;
                }
                expiration = cachePolicy.AbsoluteExpirationRelativeToNow(request, response);
                if (expiration.HasValue)
                {
                    await _cache.SetAsync(cacheKey, response, expiration.Value);
                    return response;
                }
                var expirationOffset = cachePolicy.AbsoluteExpiration(request, response);
                if (expirationOffset.HasValue)
                {
                    await _cache.SetAsync(cacheKey, response, expirationOffset.Value);
                    return response;
                }
            }
            return response;
        }
    }
}
