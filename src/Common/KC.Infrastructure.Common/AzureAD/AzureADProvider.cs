using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using NJsonSchema.Validation;
using KC.Application.Common.AzureAD;
using KC.Application.Common.Validations;
using KC.Domain.Common.AzureAD;
using KC.Utils.Common;

namespace KC.Infrastructure.Common.AzureAD
{
    public class AzureADProvider : IAzureADProvider
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger<AzureADProvider> _logger;
        private readonly AzureADSettings _settings;
        private const string _mobilePhoneId = "3179e48a-750b-4051-897c-87b9720928f7";

        public AzureADProvider(ILogger<AzureADProvider> logger, AzureADSettings settings,
            GraphServiceClient client)
        {
            _logger = logger;
            _settings = settings;
            _client = client;
        }

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // dispose resources
        }

        #endregion

        #region IAzureADProvider Methods

        public async Task<UpdateUserResponse> UpdateUserAsync(AzureADUser user,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // first get phone auth methods
                var authMethods = await _client.Users[user.Id]
                    .Authentication.PhoneMethods
                    .GetAsync(cancellationToken: cancellationToken);

                bool resetMFA = authMethods?.Value?.Count > 0 && string.IsNullOrEmpty(user.MobilePhone);

                await _client.Users[user.Id].PatchAsync(new User
                {
                    GivenName = user.FirstName,
                    Surname = user.LastName,
                    DisplayName = user.FullName,
                    Mail = user.Email.ToLower(),
                    MobilePhone = user.MobilePhone.ToB2CPhoneNumber(),
                    JobTitle = user.JobTitle,
                    Identities = new List<ObjectIdentity> {
                        new ObjectIdentity {
                            SignInType = "emailAddress",
                            Issuer = _settings.TenantId,
                            IssuerAssignedId = user.Email.ToLower()
                        }
                    },
                    AdditionalData = !resetMFA ? new Dictionary<string, object>() : new Dictionary<string, object>
                    {
                        {
                            $"extension_{_settings.B2CExtensionsAppId}_LastMFATime", ""
                        },
                        {
                            $"extension_{_settings.B2CExtensionsAppId}_LastLoginIP", ""
                        },
                        {
                            $"extension_{_settings.B2CExtensionsAppId}_MFAPreference", ""
                        },
                    }
                }, cancellationToken: cancellationToken);

                // update authentication method
                if (!string.IsNullOrEmpty(user.MobilePhone))
                {
                    // if not empty, then update, else add
                    if (authMethods?.Value?.Count > 0)
                    {
                        await _client.Users[user.Id]
                        .Authentication.PhoneMethods[_mobilePhoneId]
                        .PatchAsync(new PhoneAuthenticationMethod
                        {
                            PhoneNumber = user.MobilePhone.ToB2CPhoneNumber(),
                            PhoneType = AuthenticationPhoneType.Mobile
                        }, cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await _client.Users[user.Id]
                            .Authentication.PhoneMethods
                            .PostAsync(new PhoneAuthenticationMethod
                            {
                                PhoneNumber = user.MobilePhone.ToB2CPhoneNumber(),
                                PhoneType = AuthenticationPhoneType.Mobile
                            }, cancellationToken: cancellationToken);
                    }
                }
                else if (authMethods?.Value?.Count > 0)
                {
                    await _client.Users[user.Id]
                    .Authentication.PhoneMethods[_mobilePhoneId]
                    .DeleteAsync(cancellationToken: cancellationToken);
                }

                return UpdateUserResponse.Success;
            }
            catch (ODataError ex)
            {
                _logger.LogError(ex, "An error occurred while updating user. UserId: {id}. Email: {email}. Message: {message}.",
                    user.Id, user.Email, ex.Error?.Message);
                if (ex.ResponseStatusCode == (int)HttpStatusCode.NotFound)
                {
                    return UpdateUserResponse.Notfound;
                }
                if (ex.Error?.Message?.Contains("A conflicting object") == true)
                {
                    return UpdateUserResponse.Duplicate;
                }
                if (ex.Error?.Message?.Contains("proxyAddresses already exists") == true)
                {
                    // this occurs for users that have additional admin
                    // accounts with same email address and won't occur for regular users
                    return UpdateUserResponse.Success;
                }
                if (ex.Error?.Message?.Contains("No matching authentication method found") == true)
                {
                    return UpdateUserResponse.Success;
                }
                if (ex.Error?.Message?.Contains("The provided phone number is formatted properly, but is an invalid phone number.") == true)
                {
                    throw ValidationCodes.ValidationException("Phone", ValidationCodes.AzureAdPhoneError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user. UserId: {id}. Email: {email}",
                    user.Id, user.Email);
            }

            return UpdateUserResponse.Failed;
        }

        #endregion

    }
}
