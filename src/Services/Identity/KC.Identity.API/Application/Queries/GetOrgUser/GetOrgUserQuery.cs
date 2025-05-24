using MediatR;

namespace KC.Identity.API.Application
{
    public readonly record struct GetOrgUserQuery(int OrgId, int UserId) : IRequest<OrgUserDto>;
}
