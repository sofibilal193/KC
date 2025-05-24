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
    public record OrgRoleDto : IMap
    {
        /// <summary>
        /// Id of role.
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; init; }

        /// <summary>
        /// Disabled flag for role.
        /// </summary>
        [ProtoMember(2)]
        public bool IsDisabled { get; init; }

        /// <summary>
        /// Type of role.
        /// </summary>
        [ProtoMember(3)]
        public string Type { get; init; } = string.Empty;

        /// <summary>
        /// Name of role.
        /// </summary>
        [ProtoMember(4)]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Description of role.
        /// </summary>
        [ProtoMember(5)]
        public string? Description { get; init; }

        /// <summary>
        /// Name of user who last modified the role.
        /// </summary>
        [ProtoMember(6)]
        public string? LastModifiedUserName { get; init; }

        /// <summary>
        /// Last modified date of role in UTC time.
        /// </summary>
        [ProtoMember(7)]
        public DateTime? LastModifiedDateTimeUtc { get; init; }

        /// <summary>
        /// List of permissions for the role.
        /// </summary>
        [ProtoMember(8)]
        public List<PermissionDto>? Permissions { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Role, OrgRoleDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.LastModifiedUserName, opt => opt.MapFrom(s => s.ModifyUserName))
                .ForMember(d => d.LastModifiedDateTimeUtc, opt => opt.MapFrom(s => s.ModifyDateTimeUtc))
                .ForMember(d => d.Permissions, opt => opt.MapFrom(s => s.Permissions));
        }
    }
}

#pragma warning restore S1125
