using System.Threading;
using System.Threading.Tasks;
using MediatR;
using KC.Domain.Common;
using KC.Identity.API.Repositories;
using KC.Domain.Common.Identity;
using KC.Identity.API.Application;
using System.Linq;

namespace KC.Identity.API.Dashboard.Application
{
    public class GetDashboardUsersCountQueryHandler : IRequestHandler<GetDashboardUsersCountQuery, CountDto>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMediator _mediator;
        private readonly ICurrentUser _currentUser;

        public GetDashboardUsersCountQueryHandler(IIdentityUnitOfWork data, IMediator mediator, ICurrentUser currentUser)
        {
            _data = data;
            _mediator = mediator;
            _currentUser = currentUser;
        }

        public async Task<CountDto> Handle(GetDashboardUsersCountQuery request, CancellationToken cancellationToken)
        {
            var orgIds = await _mediator.Send(new GetCurrentUserOrgIdsQuery(false), cancellationToken);
            var result = await _data.Users.GetOrgUsersCountAsync(_currentUser.OrgType, orgIds.ToList(), cancellationToken);

            return new CountDto()
            {
                Count = result
            };
        }
    }
}
