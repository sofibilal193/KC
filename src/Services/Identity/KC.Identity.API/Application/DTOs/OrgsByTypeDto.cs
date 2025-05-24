using System;
using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using KC.Application.Common.Mappings;
using KC.Domain.Common.Identity;
using KC.Identity.API.Entities;
using ProtoBuf;

namespace KC.Identity.API.Application
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    [ProtoContract]
    public record OrgsByTypeDto : IMap
    {
        [ProtoMember(1)]
        public int Id { get; init; }

        [ProtoMember(2)]
        public int? ParentOrgId { get; init; }

        [ProtoMember(3)]
        public OrgType OrgType { get; init; }

        [ProtoMember(4)]
        public string Name { get; init; } = "";

        [ProtoMember(5)]
        public string City { get; init; } = "";

        [ProtoMember(6)]
        public string State { get; init; } = "";

        [ProtoMember(7)]
        public string Address { get; init; } = "";

        [ProtoMember(8)]
        public string ZipCode { get; init; } = "";

        [ProtoMember(9)]
        public bool IsDisabled { get; init; }

        [ProtoMember(10)]
        public string? Phone { get; init; }

        [ProtoMember(11)]
        public string? ModifyUserName { get; init; }

        [ProtoMember(12)]
        public DateTime? ModifyDateTimeUtc { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Org, OrgsByTypeDto>()
                .ForMember(d => d.ParentOrgId, opt => opt.MapFrom(s => s.GetParentOrgId()))
                .ForMember(d => d.OrgType, opt => opt.MapFrom(s => s.Type))
                .ForMember(d => d.City, opt => opt.MapFrom(s => s.PrimaryAddress == null ? null : s.PrimaryAddress.City))
                .ForMember(d => d.State, opt => opt.MapFrom(s => s.PrimaryAddress == null ? null : s.PrimaryAddress.State))
                .ForMember(d => d.Address, opt => opt.MapFrom(s => s.PrimaryAddress == null ? null : s.PrimaryAddress.Address1))
                .ForMember(d => d.ZipCode, opt => opt.MapFrom(s => s.PrimaryAddress == null ? null : s.PrimaryAddress.ZipCode))
                .ForMember(d => d.ModifyUserName, opt => opt.MapFrom(s => string.IsNullOrEmpty(s.ModifyUserName) ? s.ModifyUserName : s.CreateUserName))
                .ForMember(d => d.ModifyDateTimeUtc, opt => opt.MapFrom(s => s.ModifyDateTimeUtc.HasValue ? s.ModifyDateTimeUtc : s.CreateDateTimeUtc));
        }
    }
}
