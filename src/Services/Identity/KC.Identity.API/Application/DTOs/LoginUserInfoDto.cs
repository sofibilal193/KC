#pragma warning disable S1125
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
    public record LoginUserInfoDto : IMap
    {
        [ProtoMember(1)]
        public int Id { get; init; }

        [ProtoMember(2)]
        public string FirstName { get; init; } = "";

        [ProtoMember(3)]
        public string LastName { get; init; } = "";

        [ProtoMember(4)]
        public string Email { get; set; } = "";

        [ProtoMember(5)]
        public string? MobilePhone { get; init; }

        [ProtoMember(6)]
        public int? OrgId { get; init; }

        [ProtoMember(7)]
        public string? OrgName { get; init; }

        [ProtoMember(8)]
        public string? OrgRole { get; init; }

        [ProtoMember(9)]
        public IList<string>? Permissions { get; init; }

        [ProtoMember(10)]
        public bool IsMultipleOrgs { get; init; }

        [ProtoMember(11)]
        public IList<int> OrgIds { get; init; } = new List<int>();

        [ProtoMember(12)]
        public string? OrgTitle { get; init; }

        [ProtoMember(13)]
        public bool IsDisabled { get; init; }

        [ProtoMember(14)]
        public OrgType? OrgType { get; init; }

        [ProtoMember(15)]
        public bool IsSuperAdmin { get; private set; }

        [ProtoMember(16)]
        public bool IsMFAEnabled { get; private set; }

        [ProtoMember(17)]
        public int MFATimeOutDays { get; private set; }

        [ProtoMember(18)]
        public string UserName { get; init; } = "";

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, LoginUserInfoDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.LastName))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.MobilePhone, opt => opt.MapFrom(s => s.MobilePhone))
                .ForMember(d => d.OrgId, opt => opt.MapFrom(s => s.DefaultOrg! != null!
                    ? s.DefaultOrg.OrgId : 0))
                .ForMember(d => d.OrgName, opt => opt.MapFrom(s => s.DefaultOrg! != null!
                    && s.DefaultOrg.Org! != null! ? s.DefaultOrg.Org.Name : null))
                .ForMember(d => d.OrgRole, opt => opt.MapFrom(s => s.DefaultOrg! != null!
                    && s.DefaultOrg.Role! != null! ? s.DefaultOrg.Role.Name : null))
                .ForMember(d => d.OrgTitle, opt => opt.MapFrom(s => s.DefaultOrg! != null!
                    ? s.DefaultOrg.Title : null))
                .ForMember(d => d.IsDisabled, opt => opt.MapFrom(s => IsUserDisabled(s)))
                .ForMember(d => d.IsMultipleOrgs, opt => opt.MapFrom(s => s.Orgs != null
                    && s.Orgs.Count > 1))
                .ForMember(d => d.OrgIds, opt => opt.MapFrom(s => s.Orgs.Select(o => o.OrgId)))
                .ForMember(d => d.Permissions, opt => opt.MapFrom(s => s.DefaultOrg! != null!
                    && s.DefaultOrg.Role! != null! && s.DefaultOrg.Role.Permissions! != null!
                    ? s.DefaultOrg.Role.Permissions.Select(p => p.Name).ToList() : null))
                .ForMember(d => d.OrgType, opt => opt.MapFrom(s => s.DefaultOrg! != null!
                    && s.DefaultOrg.Org! != null! ? s.DefaultOrg.Org.Type : (OrgType?)null))
                .ForMember(d => d.IsMFAEnabled, opt => opt.Ignore())
                .ForMember(d => d.MFATimeOutDays, opt => opt.Ignore());
        }

        public void SetUserAsSuperAdmin()
        {
            IsSuperAdmin = true;
        }

        public void SetMfa(bool isEnabled, int mfaTimeOutDays)
        {
            IsMFAEnabled = isEnabled;
            MFATimeOutDays = mfaTimeOutDays;
        }

        private static bool IsUserDisabled(User u)
        {
            return u.IsDisabled || (u.DefaultOrg?.Org?.IsDisabled ?? false)
                || (u.DefaultOrg?.IsDisabled ?? false);
        }
    }
}

#pragma warning restore S1125
