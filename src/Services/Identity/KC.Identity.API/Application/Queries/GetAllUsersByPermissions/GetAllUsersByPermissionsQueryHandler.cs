using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetAllUsersByPermissionsQueryHandler : IRequestHandler<GetAllUsersByPermissionsQuery, List<PermissionUserDto>>
    {
        private readonly IIdentityUnitOfWork _data;

        public GetAllUsersByPermissionsQueryHandler(IIdentityUnitOfWork data)
        {
            _data = data;
        }
        public async Task<List<PermissionUserDto>> Handle(GetAllUsersByPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            return await _data.Users.GetAllUsersByPermissionsAsync(request.OrgId, request.Permissions, cancellationToken);
        }
    }
}
