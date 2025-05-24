using System.Collections.Generic;
using System.Linq;
using KC.Domain.Common.Entities;
using KC.Domain.Common.Events;
using KC.Domain.Common.Identity;

namespace KC.Identity.API.Entities
{
    public class Role : SqlEntity
    {
        public RoleType Type { get; private set; }

        public OrgType? OrgType { get; private set; }

        public int? OrgId { get; private set; }

        public Org? Org { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string? Description { get; private set; }

        private readonly List<Permission> _permissions;

        public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

        protected Role()
        : base()
        {
            _permissions = new List<Permission>();
        }

        public Role(RoleType type, string name, string? description, int? orgId, List<Permission> permissions) : this()
        {
            Type = type;
            Name = name;
            Description = description;
            OrgId = orgId;
            _permissions.AddRange(permissions);
            AddDomainEvent(new EntityCreatedDomainEvent<Role>(this));
        }

        public Role(RoleType type, OrgType? orgType, string name, string? description, int? orgId, List<Permission> permissions) : this()
        {
            Type = type;
            OrgType = orgType;
            Name = name;
            Description = description;
            OrgId = orgId;
            _permissions.AddRange(permissions);
            AddDomainEvent(new EntityCreatedDomainEvent<Role>(this));
        }

        public bool AddPermissions(IReadOnlyCollection<Permission> permissions)
        {
            bool saveChanges = false;
            var missingPermissions = permissions.Where(rp => !Permissions.Any(drp => drp.Name == rp.Name)).ToList();
            if (missingPermissions?.Count > 0)
            {
                saveChanges = true;
                _permissions.AddRange(missingPermissions);
            }
            return saveChanges;
        }

        public void Update(string name, string? description, List<Permission> permissions)
        {
            Name = name;
            Description = description;
            _permissions.RemoveAll(p => !permissions.Contains(p));
            _permissions.AddRange(permissions.Where(p => !_permissions.Contains(p)));
            AddDomainEvent(new EntityUpdatedDomainEvent<Role>(this));
        }

        public void ToggleStatus(bool isDisabled)
        {
            if (isDisabled)
                Disable();
            else
                Enable();
            AddDomainEvent(new EntityUpdatedDomainEvent<Role>(this));
        }
    }
}
