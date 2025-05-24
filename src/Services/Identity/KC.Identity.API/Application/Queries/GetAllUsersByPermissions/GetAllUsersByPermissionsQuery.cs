using MediatR;
using System.Collections.Generic;
using KC.Domain.Common.Types;

namespace KC.Identity.API.Application
{
    public readonly record struct GetAllUsersByPermissionsQuery : IRequest<List<PermissionUserDto>>
    {
        public int OrgId { get; init; }

        public List<string> Permissions { get; init; }
    }
}
