using FluentValidation;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace KC.Infrastructure.Common.AppInsights
{
    public class AppInsightsProcessor : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public AppInsightsProcessor(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is RequestTelemetry request &&
                (request.Url.AbsolutePath.StartsWith("/hc") ||
                 request.Url.AbsolutePath.StartsWith("/liveness") ||
                 request.Url.AbsolutePath.StartsWith("/swagger/")))
            {
                return;
            }

            if (item is DependencyTelemetry dependency &&
                dependency.ResultCode == StatusCodes.Status401Unauthorized.ToString() &&
                dependency.Data.EndsWith("hc=true"))
            {
                return;
            }

            if (item is TraceTelemetry trace &&
                trace.Properties.TryGetValue("Uri", out var uri) &&
                uri.EndsWith("hc=true"))
            {
                return;
            }

            if (item is ExceptionTelemetry ex && ex.Exception is ValidationException)
            {
                return;
            }

            _next.Process(item);
        }
    }
}
