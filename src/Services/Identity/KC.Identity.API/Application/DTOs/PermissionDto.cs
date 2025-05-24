#pragma warning disable S1125
using System;
using AutoMapper;
using KC.Application.Common.Mappings;
using KC.Identity.API.Entities;
using ProtoBuf;

namespace KC.Identity.API.Application
{
    [Serializable]
    [ProtoContract]
    public record PermissionDto : IMap
    {
        /// <summary>
        /// Id of permission.
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; init; }

        /// <summary>
        /// Category of permission.
        /// </summary>
        [ProtoMember(2)]
        public string Category { get; init; } = string.Empty;

        /// <summary>
        /// Name of permission.
        /// </summary>
        [ProtoMember(3)]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Description of permission.
        /// </summary>
        [ProtoMember(4)]
        public string? Description { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Permission, PermissionDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description));
        }
    }
}

#pragma warning restore S1125
