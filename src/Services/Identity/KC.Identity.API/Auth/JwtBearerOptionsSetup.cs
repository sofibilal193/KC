// using System;
// using System.Collections.Generic;
// using System.Security.Claims;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Options;
// using MediatR;
// using KC.Application.Common;
// using KC.Domain.Common.Constants;
// using KC.Identity.API.Application;
// using KC.Identity.API.Entities;

// namespace KC.Identity.API.Auth
// {
//     public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
//     {
//         private readonly AppSettings? _settings;
//         private readonly IServiceProvider _serviceProvider;

//         public JwtBearerOptionsSetup(IOptionsMonitor<AppSettings> settings, IServiceProvider serviceProvider)
//         {
//             _settings = settings.CurrentValue;
//             _serviceProvider = serviceProvider;
//         }

//         public void Configure(string? name, JwtBearerOptions options)
//         {
//             if (!string.IsNullOrEmpty(_settings?.AuthSettings.ClientId)) // Azure AD Client Id specified
//             {
//                 options.Events = new JwtBearerEvents()
//                 {
//                     OnTokenValidated = OnTokenValidated
//                 };
//             }
//         }

//         public void Configure(JwtBearerOptions options)
//         {
//             Configure(Options.DefaultName, options);
//         }

//         public async Task OnTokenValidated(TokenValidatedContext context)
//         {
//             var claims = new List<Claim>();

//             var providerId = context?.Principal?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
//             var userId = context?.Principal?.FindFirst("extension_UserId")?.Value;
//             var email = context?.Principal?.FindFirst(ClaimTypes.Email)?.Value ?? context?.Principal?.FindFirst("emails")?.Value;

//             // if UserId is not present, attempt to get from local db as user might have recently enrolled and claims only get refreshed in B2C if user logs out and logs back in
//             if (!string.IsNullOrEmpty(providerId) && (string.IsNullOrEmpty(userId) || userId == "0")
//                 && !string.IsNullOrEmpty(email))
//             {
//                 // Create a new scope https://andrewlock.net/the-dangers-and-gotchas-of-using-scoped-services-when-configuring-options-in-asp-net-core/
//                 using var scope = _serviceProvider.CreateScope();
//                 var mediator = scope.ServiceProvider.GetService<IMediator>();
//                 if (mediator != null)
//                 {
//                     var user = new AzureUser
//                     {
//                         AuthProvider = AuthConstants.AuthProvider,
//                         AuthProviderId = providerId,
//                         Email = email
//                     });
//                     if (user?.Id > 0)
//                     {
//                         claims.Add(new Claim("extension_UserId", user.Id.ToString()));
//                     }
//                 }
//             }
//         }
//     }
// }
