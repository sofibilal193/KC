using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KC.Application.Common.Mappings;
using KC.Domain.Common.Identity;
using KC.Identity.API.Entities;
using KC.Utils.Common;
using ProtoBuf;

namespace KC.Identity.API.Application
{
    [Serializable]
    [ProtoContract]
    public record SearchUserDto : IMap
    {
        [ProtoMember(1)]
        public int Id { get; init; }

        [ProtoMember(2)]
        public string FirstName { get; init; } = "";

        [ProtoMember(3)]
        public string LastName { get; init; } = "";

        [ProtoMember(4)]
        public string FullName { get; init; } = "";

        [ProtoMember(5)]
        public string Email { get; init; } = "";

        [ProtoMember(6)]
        public string MobilePhone { get; init; } = "";

        [ProtoMember(7)]
        public DateTime? LastLoginDateTimeUtc { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, SearchUserDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.LastName))
                .ForMember(d => d.FullName, opt => opt.MapFrom(s => StringUtil.Join(' ', s.FirstName, !String.IsNullOrEmpty(s.LastName) ? s.LastName : "")))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.MobilePhone, opt => opt.MapFrom(s => s.MobilePhone))
                .ForMember(d => d.LastLoginDateTimeUtc, opt => opt.MapFrom(s => s.LastLoginDateTimeUtc));
        }
    }
}
