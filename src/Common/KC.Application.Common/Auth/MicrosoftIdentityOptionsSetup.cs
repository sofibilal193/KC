using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace KC.Application.Common.Auth
{
    public class MicrosoftIdentityOptionsSetup : IConfigureNamedOptions<MicrosoftIdentityOptions>
    {
        private readonly AppSettings? _settings;

        public MicrosoftIdentityOptionsSetup(IOptionsMonitor<AppSettings> settings)
        {
            _settings = settings.CurrentValue;
        }

        public void Configure(string? name, MicrosoftIdentityOptions options)
        {
            if (_settings?.AuthSettings.Type == ApiAuthType.AzureAdB2C &&
                !string.IsNullOrEmpty(_settings?.AuthSettings.ClientId) &&
                !string.IsNullOrEmpty(_settings?.AuthSettings.Domain))
            {
                options.Instance = _settings.AuthSettings.Instance;
                options.ClientId = _settings.AuthSettings.ClientId;
                options.Domain = _settings.AuthSettings.Domain;
                options.SignUpSignInPolicyId = _settings.AuthSettings.SignUpSignInPolicyId;
                options.ResetPasswordPolicyId = _settings.AuthSettings.ResetPasswordPolicyId;
                options.EditProfilePolicyId = _settings.AuthSettings.EditProfilePolicyId;
                options.TokenValidationParameters.NameClaimType = "name";
            }
            else if (_settings?.AuthSettings.Type == ApiAuthType.AzureAd &&
                !string.IsNullOrEmpty(_settings?.AuthSettings.ClientId) &&
                !string.IsNullOrEmpty(_settings?.AuthSettings.TenantId))
            {
                options.Instance = _settings.AuthSettings.Instance;
                options.ClientId = _settings.AuthSettings.ClientId;
                options.TenantId = _settings.AuthSettings.TenantId;
            }
        }

        public void Configure(MicrosoftIdentityOptions options)
        {
            Configure(Options.DefaultName, options);
        }
    }
}
