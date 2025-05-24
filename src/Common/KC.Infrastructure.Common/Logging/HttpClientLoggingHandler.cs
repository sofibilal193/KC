using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace KC.Infrastructure.Common.Logging
{
    public class HttpClientLoggingHandler : DelegatingHandler
    {
        #region Private Variables

        private readonly IHostEnvironment _env;
        private readonly ILogger<HttpClientLoggingHandler> _logger;
        private readonly IFeatureManager _featureManager;

        #endregion

        #region Constructors

        public HttpClientLoggingHandler(IHostEnvironment env, ILogger<HttpClientLoggingHandler> logger, IFeatureManager featureManager)
            : base()
        {
            _env = env;
            _logger = logger;
            _featureManager = featureManager;
        }

        #endregion

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool enabled;
            if (_env.IsDevelopment())
            {
                enabled = true;
            }
            else
            {
                enabled = await _featureManager.IsEnabledAsync($"httpclient-logging-{request.RequestUri?.Authority?.ToLower()}", cancellationToken);
            }

            if (enabled)
            {
                _logger.LogInformation("Request:{newLine}{request}", Environment.NewLine,
                    request.ToString());
                if (request.Content != null)
                {
                    _logger.LogInformation("Request Content:{newLine}{content}", Environment.NewLine,
                        await request.Content.ReadAsStringAsync(cancellationToken));
                }
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (enabled)
            {
                _logger.LogInformation("Response:{newLine}{response}", Environment.NewLine,
                    response.ToString());
                if (response.Content != null)
                {
                    _logger.LogInformation("Response Content:{newLine}{content}", Environment.NewLine,
                        await response.Content.ReadAsStringAsync(cancellationToken));
                }
            }

            return response;
        }
    }
}
