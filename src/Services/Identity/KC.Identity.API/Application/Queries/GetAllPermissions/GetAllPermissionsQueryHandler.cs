using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, List<PermissionDto>>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMapper _mapper;

        public GetAllPermissionsQueryHandler(IIdentityUnitOfWork data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<List<PermissionDto>> Handle(GetAllPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            var permissionAll = await _data.Permissions.AsNoTracking().GetAsync(c => !c.IsDisabled,
                cancellationToken);
            return _mapper.Map<List<PermissionDto>>(permissionAll);
        }
    }
}
