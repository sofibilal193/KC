using System.Collections.Generic;
using MediatR;

namespace KC.Identity.API.Application
{
    public readonly record struct GetAllPermissionsQuery() : IRequest<List<PermissionDto>>;
}
