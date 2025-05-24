using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using KC.Application.Common.Validations;
using KC.Identity.API.Application.DTOs;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application.Queries.GetUserInvitation
{
    public class GetUserInvitationQueryHandler : IRequestHandler<GetUserInvitationQuery, UserInvitationDto?>
    {
        private readonly IIdentityUnitOfWork _data;

        public GetUserInvitationQueryHandler(IIdentityUnitOfWork data)
        {
            _data = data;
        }

        public async Task<UserInvitationDto?> Handle(GetUserInvitationQuery request, CancellationToken cancellationToken)
        {
            var user = await _data.Users.AsNoTracking()
                .Include(u => u.Orgs.Where(o => o.Id == request.OrgUserId)).Include("Orgs.Org")
                .FirstOrDefaultAsync(u => !u.IsDisabled && u.Orgs.Any(o => o.Id == request.OrgUserId
                    && o.IsInvited && !o.IsDisabled), cancellationToken);

            if (user is null)
            {
                throw ValidationCodes.ValidationException("inviteId", ValidationCodes.InvitationNotFound);
            }

            var orgUser = user.Orgs.Single();
            if (orgUser.IsInviteProcessed)
            {
                throw ValidationCodes.ValidationException("inviteId", ValidationCodes.InvitationAlreadyAccepted);
            }

            var org = orgUser.Org!;
            var inviteUser = await _data.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == orgUser.CreateUserId, cancellationToken);

            return new UserInvitationDto
            {
                InviteUserName = inviteUser?.FullName ?? "",
                InviteUserEmail = inviteUser?.Email ?? "",
                FullName = user.FullName,
                OrgId = org.Id,
                OrgName = org.Name,
                OrgAddress = org.PrimaryAddress!.Address1,
                OrgCity = org.PrimaryAddress.City,
                OrgState = org.PrimaryAddress.State,
                OrgZipCode = org.PrimaryAddress.ZipCode,
                OrgPhone = org.Phone
            };
        }
    }
}
