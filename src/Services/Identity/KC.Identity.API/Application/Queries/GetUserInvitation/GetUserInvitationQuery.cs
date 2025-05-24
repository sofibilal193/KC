using MediatR;
using KC.Identity.API.Application.DTOs;

namespace KC.Identity.API.Application.Queries.GetUserInvitation
{
    public readonly record struct GetUserInvitationQuery(int OrgUserId) : IRequest<UserInvitationDto>;
}
