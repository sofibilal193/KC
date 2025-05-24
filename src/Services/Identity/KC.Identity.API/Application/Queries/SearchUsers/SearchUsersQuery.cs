using System.Collections.Generic;
using MediatR;
using KC.Domain.Common.Identity;

namespace KC.Identity.API.Application
{
    public readonly record struct SearchUsersQuery(string Search)
        : IRequest<List<SearchUserDto>>;
}
