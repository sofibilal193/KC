using MediatR;

namespace KC.Identity.API.Application.Queries
{
    public readonly record struct GetFreshDeskTokenQuery(int UserId) : IRequest<string?>;
}
