using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using KC.Domain.Common;
using KC.Domain.Common.Identity;
using KC.Identity.API.Application;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Dashboard.Application
{
    public class GetDashboardOrgsCountQueryHandler : IRequestHandler<GetDashboardOrgsCountQuery, CountDto>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMediator _mediator;
        private readonly ICurrentUser _currentUser;

        public GetDashboardOrgsCountQueryHandler(IIdentityUnitOfWork data, IMediator mediator, ICurrentUser currentUser)
        {
            _data = data;
            _mediator = mediator;
            _currentUser = currentUser;
        }

        public async Task<CountDto> Handle(GetDashboardOrgsCountQuery request, CancellationToken cancellationToken)
        {
            int count;
            if (_currentUser.OrgType == OrgType.SuperAdmin)
            {
                count = await _data.Orgs.GetOrgCountAsync(cancellationToken);
            }
            else
            {
                var orgIds = await _mediator.Send(new GetCurrentUserOrgIdsQuery(false), cancellationToken);
                count = orgIds.Count();
            }

            return new CountDto()
            {
                Count = count
            };
        }
    }
}
