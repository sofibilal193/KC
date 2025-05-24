#pragma warning disable S1125
using System;
using System.Collections.Generic;
using AutoMapper;
using KC.Application.Common.Mappings;
using KC.Identity.API.Entities;
using ProtoBuf;

namespace KC.Identity.API.Application
{
    [Serializable]
    [ProtoContract]
    public record RoleDto : IMap
    {
        /// <summary>
        /// Id of role.
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; init; }

        /// <summary>
        /// Name of role.
        /// </summary>
        [ProtoMember(2)]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Description of role.
        /// </summary>
        [ProtoMember(3)]
        public string? Description { get; init; }

        /// <summary>
        /// List of permissions for the role.
        /// </summary>
        [ProtoMember(4)]
        public IList<PermissionDto>? Permissions { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Role, RoleDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.Permissions, opt => opt.MapFrom(s => s.Permissions));
        }
    }
}

#pragma warning restore S1125
