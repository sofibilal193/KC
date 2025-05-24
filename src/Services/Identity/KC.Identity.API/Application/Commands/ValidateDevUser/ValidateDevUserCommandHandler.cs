using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Newtonsoft.Json;
using KC.Application.Common;
using KC.Application.Common.Auth;
using KC.Domain.Common;
using KC.Domain.Common.Exceptions;
using KC.Domain.Common.Identity;
using KC.Identity.API.Entities;
using KC.Identity.API.Notifications;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class ValidateDevUserCommandHandler : IRequestHandler<ValidateDevUserCommand,
        ValidateDevUserResponse>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMediator _mediator;
        private readonly AppSettings _settings;
        private readonly IDateTime _dt;
        private readonly ICurrentUser _currentUser;

        public ValidateDevUserCommandHandler(IOptions<AppSettings> settingsOptions,
            IIdentityUnitOfWork data, IMediator mediator, IDateTime dt, ICurrentUser currentUser)
        {
            _data = data;
            _mediator = mediator;
            _settings = settingsOptions.Value;
            _dt = dt;
            _currentUser = currentUser;
        }

        public async Task<ValidateDevUserResponse> Handle(ValidateDevUserCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_settings.AuthSettings.DevTokenSecretKey))
            {
                throw new DomainException("DevTokenSecretKey has not been set.");
            }
            if (string.IsNullOrEmpty(_settings.AuthSettings.DevTokenIssuer))
            {
                throw new DomainException("DevTokenIssuer has not been set.");
            }
            var path = Path.GetFullPath("Seed/dev_users.json");
            var users = JsonConvert.DeserializeObject<List<DevUser>>(await File.ReadAllTextAsync(path, cancellationToken));
            if (users is null)
            {
                return default;
            }

            var user = users.Find(u => u.Email.ToLower() == request.Email.ToLower()
                && u.Password == request.Password);
            if (user == default)
            {
                return default;
            }

            var devUser = await _data.Users.Include(u => u.Orgs)
                .FirstOrDefaultAsync(u => u.Email == user.Email, cancellationToken);
            if (devUser is null)
            {
                devUser = new User(user.FirstName, user.LastName, user.Email, null);
                devUser.UpsertOrg(1, 1, true, "Developer");
                _data.Users.Add(devUser);
                await _data.SaveEntitiesAsync(cancellationToken);
            }

            //Create a List of Claims, Keep claims name short
            var permClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email.ToLower()),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("extension_UserId", devUser.Id.ToString()),
                new Claim("extension_OrgIds", string.Join(',', devUser.Orgs.Select(o => o.OrgId))),
                new Claim("extension_IsSuperAdmin", devUser.Orgs.Any(o => o.OrgId == 1).ToString())
            };

            //Create Security Token object by giving required parameters
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.AuthSettings.DevTokenSecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_settings.AuthSettings.DevTokenIssuer, // Issuer
                _settings.AuthSettings.DevTokenIssuer,  //Audience
                permClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);

            // log event
            var log = new IdentityEventLogNotification("dev user login",
                _dt.Now, _currentUser.Source,
                $"Name: {user.FullName}. Email: {user.Email}.", _currentUser.UserId,
                null, null);
            await _mediator.Publish(log, cancellationToken);

            return new ValidateDevUserResponse(jwt_token);
        }
    }
}
