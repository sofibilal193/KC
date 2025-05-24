using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KC.Application.Common.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ILogger<BasicAuthenticationHandler> _logger;
        private readonly AppSettings _settings;

        public BasicAuthenticationHandler(
           IOptionsMonitor<AuthenticationSchemeOptions> options,
           ILoggerFactory loggerFactory,
           UrlEncoder encoder,
           IOptions<AppSettings> settingsOptions)
           : base(options, loggerFactory, encoder)
        {
            _settings = settingsOptions.Value;
            _logger = loggerFactory.CreateLogger<BasicAuthenticationHandler>();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // skip authentication if endpoint has [AllowAnonymous] attribute
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return Task.FromResult(AuthenticateResult.NoResult());

            if (string.IsNullOrEmpty(_settings?.AuthSettings.BasicAuthUserName))
            {
                _logger.LogError("Settings.AuthSettings.BasicAuthUserName is missing.");
                return Task.FromResult(AuthenticateResult.Fail("Settings.AuthSettings.BasicAuthUserName is missing."));
            }

            if (string.IsNullOrEmpty(_settings.AuthSettings.BasicAuthPassword))
            {
                _logger.LogError("Settings.AuthSettings.BasicAuthPassword is missing.");
                return Task.FromResult(AuthenticateResult.Fail("Settings.AuthSettings.BasicAuthPassword is missing."));
            }

            if (!Request.Headers.TryGetValue("Authorization", out var authorization))
                return Task.FromResult(AuthenticateResult.Fail("Authorization header is missing."));

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(authorization!);
                if (authHeader?.Parameter != null)
                {
                    var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                    var username = credentials[0];
                    var password = credentials[1];
                    if (username == _settings.AuthSettings.BasicAuthUserName && password == _settings.AuthSettings.BasicAuthPassword)
                    {
                        var claims = new[] {
                            new Claim(ClaimTypes.Name, username),
                        };
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);

                        return Task.FromResult(AuthenticateResult.Success(ticket));
                    }
                }
                else
                {
                    _logger.LogError("Error validating credentials.");
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating credentials.");
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header."));
            }

            return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));
        }
    }
}
