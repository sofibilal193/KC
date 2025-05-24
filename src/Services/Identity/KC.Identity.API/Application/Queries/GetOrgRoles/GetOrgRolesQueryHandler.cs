using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using KC.Identity.API.Entities;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetOrgRolesQueryHandler : IRequestHandler<GetOrgRolesQuery, List<OrgRoleDto>>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMapper _mapper;

        public GetOrgRolesQueryHandler(IIdentityUnitOfWork data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<List<OrgRoleDto>> Handle(GetOrgRolesQuery request,
            CancellationToken cancellationToken)
        {
            var org = await _data.Orgs.AsNoTracking().GetAsync(request.OrgId, cancellationToken);
            var orgType = org?.Type;

            var roles = await _data.Roles.AsNoTracking().Include(r => r.Permissions.Where(p => !p.IsDisabled)
                .OrderBy(p => p.Category)).GetAsync(r => (r.Type == RoleType.Standard && r.OrgType == orgType)
                || (r.Type == RoleType.Custom && r.OrgId == request.OrgId), cancellationToken);
            return _mapper.Map<List<OrgRoleDto>>(roles);
        }
    }
}
