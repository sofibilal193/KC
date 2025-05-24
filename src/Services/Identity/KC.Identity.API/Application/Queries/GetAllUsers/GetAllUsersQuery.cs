using MediatR;
using KC.Domain.Common.Types;

namespace KC.Identity.API.Application
{
    public readonly record struct GetAllUsersQuery(int PageNumber, int PageSize, string? Search, string? Sort) : IRequest<PagedList<UserDto>>;
}
