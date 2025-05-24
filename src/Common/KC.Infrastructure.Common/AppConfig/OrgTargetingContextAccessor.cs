using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement.FeatureFilters;

namespace KC.Infrastructure.Common.AppConfig
{
    public class OrgTargetingContextAccessor : ITargetingContextAccessor
    {
        private const string _targetingContextLookup = "OrgTargetingContextAccessor.TargetingContext";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrgTargetingContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public ValueTask<TargetingContext> GetContextAsync()
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                if (httpContext.Items.TryGetValue(_targetingContextLookup, out object? value) && value is not null)
                {
                    return new ValueTask<TargetingContext>((TargetingContext)value);
                }

                var routeValues = httpContext.Request.RouteValues;
                if (routeValues.TryGetValue("orgId", out var orgId))
                {
                    var id = orgId?.ToString();
                    if (!string.IsNullOrEmpty(id))
                    {
                        TargetingContext targetingContext = new TargetingContext
                        {
                            Groups = new List<string> {
                                string.Concat("OrgId:", id)
                            }
                        };
                        return new ValueTask<TargetingContext>(targetingContext);
                    }
                }
            }

            return new ValueTask<TargetingContext>(new TargetingContext());
        }
    }
}
