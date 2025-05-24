using System.Collections.Generic;
using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using KC.Identity.API.Entities;

namespace KC.Identity.API.Application.Commands
{
    public record UpsertRolePermissionCommand : IRequest<int>
    {
        [JsonIgnore]
        public int OrgId { get; private set; }

        [JsonIgnore]
        public int? Id { get; private set; }

        [JsonIgnore]
        public RoleType Type { get; init; }

        /// <summary>
        /// Name of role.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Description of role.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// List of permissions for role.
        /// </summary>
        public List<int> PermissionIds { get; init; } = new();

        public void SetIds(int orgId, int? id = null)
        {
            OrgId = orgId;
            Id = id;
        }
    }

    public class UpsertRolePermissionCommandValidator : AbstractValidator<UpsertRolePermissionCommand>
    {
        public UpsertRolePermissionCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty();
        }
    }
}
