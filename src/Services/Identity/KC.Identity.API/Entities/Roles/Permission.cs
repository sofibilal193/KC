using System.Collections.Generic;
using KC.Domain.Common.Entities;

namespace KC.Identity.API.Entities
{
    public class Permission : SqlEntity
    {
        public string Category { get; private set; }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        private readonly List<Role> _roles;

        public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

        public Permission(string category, string name, string? description)
        {
            _roles = new List<Role>();
            Category = category;
            Name = name;
            Description = description;
        }
    }
}
