#pragma warning disable
using System;
using System.Collections.Generic;
using AutoMapper;
using KC.Application.Common.Mappings;
using KC.Identity.API.Entities;
using KC.Utils.Common;
using ProtoBuf;

namespace KC.Identity.API.Application
{
    [Serializable]
    [ProtoContract]
    public record UserDto : IMap
    {
        /// <summary>
        /// Id of role.
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; init; }

        /// <summary>
        /// First Name of user.
        /// </summary>
        [ProtoMember(2)]
        public string FirstName { get; init; } = string.Empty;

        /// <summary>
        /// Last Name of user.
        /// </summary>
        [ProtoMember(3)]
        public string LastName { get; init; } = string.Empty;

        /// <summary>
        /// Full Name of user.
        /// </summary>
        [ProtoMember(4)]
        public string FullName { get; init; } = string.Empty;

        /// <summary>
        /// Email of user.
        /// </summary>
        [ProtoMember(5)]
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Mobile Phone Number of user.
        /// </summary>
        [ProtoMember(6)]
        public string MobilePhone { get; init; } = string.Empty;

        /// <summary>
        /// Last Login Date Time of user.
        /// </summary>
        [ProtoMember(7)]
        public DateTime? LastLoginDateTimeUtc { get; init; }

        /// <summary>
        /// Last Modified Date Time of user.
        /// </summary>
        [ProtoMember(8)]
        public DateTime? LastModifiedDateTimeUtc { get; init; }

        /// <summary>
        /// Last Modified Date Time of user.
        /// </summary>
        [ProtoMember(8)]
        public bool IsDisabled { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.LastName))
                .ForMember(d => d.FullName, opt => opt.MapFrom(s => StringUtil.Join(' ', s.FirstName, !String.IsNullOrEmpty(s.LastName) ? s.LastName : "")))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.MobilePhone, opt => opt.MapFrom(s => s.MobilePhone))
                .ForMember(d => d.LastLoginDateTimeUtc, opt => opt.MapFrom(s => s.LastLoginDateTimeUtc))
                .ForMember(d => d.LastModifiedDateTimeUtc, opt => opt.MapFrom(s => s.ModifyDateTimeUtc))
                .ForMember(d => d.IsDisabled, opt => opt.MapFrom(s => s.IsDisabled));

        }
    }
}

#pragma warning restore S1125
