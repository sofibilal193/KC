using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KC.Application.Common.Auth
{
    public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly AppSettings? _settings;

        public JwtBearerOptionsSetup(IOptionsMonitor<AppSettings> settings)
        {
            _settings = settings.CurrentValue;
        }

        public void Configure(string? name, JwtBearerOptions options)
        {
            if (_settings?.AuthSettings.Type == ApiAuthType.JWT && !string.IsNullOrEmpty(_settings?.AuthSettings.DevTokenSecretKey))
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings.AuthSettings.DevTokenIssuer,
                    ValidAudience = _settings.AuthSettings.DevTokenIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.AuthSettings.DevTokenSecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                        }
                        return Task.CompletedTask;
                    }
                };
            }
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(Options.DefaultName, options);
        }

        public static Task OnTokenValidated(TokenValidatedContext context)
        {
            var claims = new List<Claim>();

            // add permissions as role claims
            var perms = context?.Principal?.FindFirst("extension_OrgPermissions")?.Value;
            if (!string.IsNullOrEmpty(perms))
            {
                claims.AddRange(perms.Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => new Claim(ClaimTypes.Role, p)));
            }

            // add role as role claim
            var role = context?.Principal?.FindFirst("extension_OrgRole")?.Value;
            if (!string.IsNullOrEmpty(role))
                claims.Add(new Claim(ClaimTypes.Role, role));

            if (claims.Count > 0)
            {
                var appIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                context?.Principal?.AddIdentity(appIdentity);
            }

            return Task.CompletedTask;
        }
    }
}
