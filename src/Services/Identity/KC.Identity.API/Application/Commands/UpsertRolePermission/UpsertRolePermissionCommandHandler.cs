using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using KC.Application.Common.Validations;
using KC.Domain.Common.Exceptions;
using KC.Identity.API.Entities;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application.Commands.UpsertRolePermission
{
    public class UpsertRolePermissionCommandHandler : IRequestHandler<UpsertRolePermissionCommand, int>
    {
        private readonly IIdentityUnitOfWork _data;

        public UpsertRolePermissionCommandHandler(IIdentityUnitOfWork data)
        {
            _data = data;
        }

        public async Task<int> Handle(UpsertRolePermissionCommand request, CancellationToken cancellationToken)
        {
            Role? role = null;
            if (request.Id.HasValue)
            {
                role = await _data.Roles.Include(r => r.Permissions)
                    .FirstOrDefaultAsync(d => d.OrgId == request.OrgId && d.Id == request.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Role), request.Id);
            }

            // duplicate name check
            var existingRole = await _data.Roles.FirstOrDefaultAsync(r => (r.Type == RoleType.Standard
                || (r.Type == RoleType.Custom && r.OrgId == request.OrgId))
                && r.Id != request.Id &&
                r.Name == request.Name, cancellationToken);
            if (existingRole is not null)
            {
                throw ValidationCodes.ValidationException(nameof(request.Name), ValidationCodes.RoleAlreadyExists);
            }

            var permissions = role?.Permissions?.ToList() ?? new List<Permission>();
            permissions.RemoveAll(p => !request.PermissionIds.Contains(p.Id));
            var missingPermissionIds = request.PermissionIds.Where(id => permissions.Find(p => p.Id == id) is null);
            if (missingPermissionIds.Any())
            {
                var missingPermissions = await _data.Permissions.GetAsync(p => missingPermissionIds.Contains(p.Id), cancellationToken);
                permissions.AddRange(missingPermissions);
            }

            if (role is null)
            {
                role = new Role(RoleType.Custom, request.Name, request.Description, request.OrgId, permissions);
                _data.Roles.Add(role);
            }
            else
            {
                role.Update(request.Name, request.Description, permissions);
            }

            await _data.SaveEntitiesAsync(cancellationToken);
            return role.Id;
        }
    }
}
