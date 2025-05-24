using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KC.Application.Common.Mappings;
using KC.Domain.Common.Identity;
using KC.Identity.API.Entities;
using ProtoBuf;

namespace KC.Identity.API.Application
{
    [Serializable]
    [ProtoContract]
    public record UserOrgDto : IMap
    {
        [ProtoMember(1)]
        public int? Id { get; init; }

        [ProtoMember(2)]
        public string Name { get; init; } = "";

        [ProtoMember(3)]
        public OrgType? OrgType { get; init; }

        [ProtoMember(4)]
        public string Address { get; init; } = "";

        [ProtoMember(5)]
        public string? SuiteNo { get; init; }

        [ProtoMember(6)]
        public string City { get; init; } = "";

        [ProtoMember(7)]
        public string State { get; init; } = "";

        [ProtoMember(8)]
        public string ZipCode { get; init; } = "";

        [ProtoMember(9)]
        public DateTime? ModifyDateTimeUtc { get; init; }

        [ProtoMember(10)]
        public string? OrgRole { get; init; }

        [ProtoMember(11)]
        public IList<string>? Permissions { get; init; }

        [ProtoMember(12)]
        public bool IsDefault { get; init; }

        [ProtoMember(13)]
        public bool IsDisabled { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OrgUser, UserOrgDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Org!.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Org!.Name))
                .ForMember(d => d.OrgType, opt => opt.MapFrom(s => s.Org!.Type))
                .ForMember(d => d.Address, opt => opt.MapFrom(s => s.Org!.PrimaryAddress == null ? null : s.Org!.PrimaryAddress.Address1))
                .ForMember(d => d.SuiteNo, opt => opt.MapFrom(s => s.Org!.PrimaryAddress == null ? null : s.Org!.PrimaryAddress.Address2))
                .ForMember(d => d.City, opt => opt.MapFrom(s => s.Org!.PrimaryAddress == null ? null : s.Org!.PrimaryAddress.City))
                .ForMember(d => d.State, opt => opt.MapFrom(s => s.Org!.PrimaryAddress == null ? null : s.Org!.PrimaryAddress.State))
                .ForMember(d => d.ZipCode, opt => opt.MapFrom(s => s.Org!.PrimaryAddress == null ? null : s.Org!.PrimaryAddress.ZipCode))
                .ForMember(d => d.OrgRole, opt => opt.MapFrom(s => s.Role! != null! ? s.Role.Name : null))
                .ForMember(d => d.Permissions, opt => opt.MapFrom(s => s.Role!.Permissions! != null!
                    && s.Role! != null! && s.Role.Permissions! != null!
                    ? s.Role.Permissions.Select(p => p.Name).ToList() : null))
                .ForMember(d => d.IsDisabled, opt => opt.MapFrom(s => IsOrgUserDisabled(s)));
        }

        private static bool IsOrgUserDisabled(OrgUser orgUser)
        {
            return orgUser.IsDisabled || orgUser.Org!.IsDisabled;
        }
    }
}
