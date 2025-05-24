using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KC.Domain.Common.Constants;

namespace KC.Application.Common.Auth
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AppSettings _settings;
        private readonly ILogger<ApiKeyAuthenticationHandler> _logger;

        public ApiKeyAuthenticationHandler(
           IOptionsMonitor<AuthenticationSchemeOptions> options,
           ILoggerFactory loggerFactory,
           UrlEncoder encoder,
           IOptions<AppSettings> settingsOptions)
           : base(options, loggerFactory, encoder)
        {
            _settings = settingsOptions.Value;
            _logger = loggerFactory.CreateLogger<ApiKeyAuthenticationHandler>();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // check if authorization header is present and if so, skip api-key auth
            if (Request.Headers.Authorization.Count > 0)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (!Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("Api Key was not provided"));
            }

            var apiKey = _settings.AuthSettings.ApiKey;
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("Settings.AuthSettings.ApiKey is missing.");
                return Task.FromResult(AuthenticateResult.Fail("Settings.AuthSettings.ApiKey is missing."));
            }

            if (apiKey.Equals(extractedApiKey, StringComparison.Ordinal))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "ApiClient")
                };
                if (Request.RouteValues.TryGetValue("orgId", out var value) && int.TryParse(value?.ToString(), out var orgId))
                {
                    claims.Add(new Claim("extension_OrgIds", orgId.ToString()));
                }
                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            return Task.FromResult(AuthenticateResult.Fail("Api Key is not valid"));
        }
    }
}
