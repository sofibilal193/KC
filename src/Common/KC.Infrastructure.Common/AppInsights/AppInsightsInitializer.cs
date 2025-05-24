using System;
using System.Security.Claims;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace KC.Infrastructure.Common.AppInsights
{
    public class AppInsightsInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppInsightsInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Initialize(ITelemetry telemetry)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is not null)
            {
                if (httpContext.User?.Identity?.IsAuthenticated == true)
                {
                    telemetry.Context.User.AuthenticatedUserId = httpContext.User.FindFirst(ClaimTypes.Sid)?.Value;
                }

                if (httpContext.Request?.Headers?.TryGetValue("Session-Id", out var sessionId) == true
                    && !string.IsNullOrEmpty(sessionId))
                {
                    telemetry.Context.Session.Id = sessionId;
                }
            }
        }
    }
}
