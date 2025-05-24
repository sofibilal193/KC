using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using KC.Identity.API.Repositories;
using System.Collections.Generic;
using KC.Identity.API.Entities;
using System.Linq;

namespace KC.Identity.API.Application
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IList<RoleDto>>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMapper _mapper;

        public GetRolesQueryHandler(IIdentityUnitOfWork data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<IList<RoleDto>> Handle(GetRolesQuery request,
            CancellationToken cancellationToken)
        {
            // attempt to get roles from db
            var roles = await _data.Roles.AsNoTracking().Include(r => r.Permissions.Where(p => !p.IsDisabled)
                .OrderBy(p => p.Category)).GetAsync(r => !r.IsDisabled && !request.OrgId.HasValue ? r.Type == RoleType.Standard :
                    (r.Type == RoleType.Standard || r.OrgId == request.OrgId), cancellationToken);
            return _mapper.Map<IList<RoleDto>>(roles);
        }
    }
}
