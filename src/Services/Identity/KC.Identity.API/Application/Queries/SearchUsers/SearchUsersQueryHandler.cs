using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, List<SearchUserDto>>
    {
        private readonly IIdentityUnitOfWork _data;

        public SearchUsersQueryHandler(IIdentityUnitOfWork data)
        {
            _data = data;
        }

        public async Task<List<SearchUserDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            return await _data.Users.SearchAsync(request.Search, cancellationToken);
        }
    }
}
