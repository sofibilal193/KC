using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using KC.Domain.Common.Identity;
using KC.Domain.Common.Types;
using KC.Identity.API.Entities;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetOrgsByOrgTypeQueryHandler : IRequestHandler<GetOrgsByOrgTypeQuery, PagedList<OrgsByTypeDto?>>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public GetOrgsByOrgTypeQueryHandler(IIdentityUnitOfWork data, IMapper mapper, ICurrentUser currentUser)
        {
            _data = data;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<PagedList<OrgsByTypeDto?>> Handle(GetOrgsByOrgTypeQuery request,
            CancellationToken cancellationToken)
        {
            var sortExpression = request.Sort ?? $"-{nameof(Org.CreateDateTimeUtc)}";
            var orgs = await _data.Orgs.GetUserOrgsAsync(_currentUser.OrgType, _currentUser.OrgIds,
                request.ParentOrgId, request.Page, request.PageSize, request.Search, sortExpression);
            return _mapper.Map<PagedList<OrgsByTypeDto?>>(orgs);
        }
    }
}
