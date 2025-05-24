using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using KC.Domain.Common.Identity;
using KC.Identity.API.Entities;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetOrgPermissionsQueryHandler : IRequestHandler<GetOrgPermissionsQuery, List<PermissionDto>>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMapper _mapper;

        public GetOrgPermissionsQueryHandler(IIdentityUnitOfWork data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<List<PermissionDto>> Handle(GetOrgPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            var orgType = await _data.Orgs.GetOrgTypeAsync(request.OrgId, cancellationToken);

            var permissions = await _data.Roles.AsNoTracking().Include(r => r.Permissions.Where(p => !p.IsDisabled)
                .OrderBy(p => p.Category)).GetAsync(r => (r.Type == RoleType.Standard && r.OrgType == orgType)
                || (r.Type == RoleType.Custom && r.OrgId == request.OrgId), cancellationToken);
            return _mapper.Map<List<PermissionDto>>(permissions.SelectMany(i => i.Permissions).Distinct());
        }
    }
}
