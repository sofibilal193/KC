#pragma warning disable S1125
using System;
using AutoMapper;
using KC.Application.Common.Mappings;
using KC.Domain.Common.Identity;
using KC.Identity.API.Entities;
using ProtoBuf;

namespace KC.Identity.API.Application
{
    [Serializable]
    [ProtoContract]
    public record OrgUserDto : IMap
    {
        /// <summary>
        /// Id of user.
        /// </summary>
        [ProtoMember(1)]
        public int? Id { get; init; }

        /// <summary>
        /// User's FirstName.
        /// </summary>
        [ProtoMember(2)]
        public string FirstName { get; init; } = "";

        /// <summary>
        /// User's LastName.
        /// </summary>
        [ProtoMember(3)]
        public string LastName { get; init; } = "";

        /// <summary>
        /// User's FullName.
        /// </summary>
        [ProtoMember(4)]
        public string FullName { get; init; } = "";

        /// <summary>
        /// User's Email.
        /// </summary>
        [ProtoMember(5)]
        public string Email { get; set; } = "";

        /// <summary>
        /// User's CellPhone.
        /// </summary>
        [ProtoMember(6)]
        public string MobilePhone { get; set; } = "";

        /// <summary>
        /// User's JobTitle.
        /// </summary>
        [ProtoMember(7)]
        public string Title { get; set; } = "";

        /// <summary>
        /// User's role for the organization.
        /// </summary>
        [ProtoMember(8)]
        public string? OrgRole { get; init; }

        /// <summary>
        ///User's roleid for the organization.
        /// </summary>
        [ProtoMember(9)]
        public int OrgRoleId { get; init; }

        /// <summary>
        /// User's last login date/time in UTC time.
        /// </summary>
        [ProtoMember(10)]
        public DateTime? LastLoginDateTimeUtc { get; init; }

        /// <summary>
        ///User's last modified date/time in UTC time.
        /// </summary>
        [ProtoMember(11)]
        public DateTime LastModifiedDateTimeUtc { get; init; }

        /// <summary>
        ///Is User disabled.
        /// </summary>
        [ProtoMember(12)]
        public bool IsDisabled { get; init; }

        /// <summary>
        ///Is User invitation pending.
        /// </summary>
        [ProtoMember(13)]
        public bool IsInvitationPending { get; init; }

        /// <summary>
        ///User's Invitation sent date/time in UTC time.
        /// </summary>
        [ProtoMember(14)]
        public DateTime UserInviteSentDateTimeUtc { get; init; }

        /// <summary>
        ///Type of organization.
        /// </summary>
        [ProtoMember(14)]
        public OrgType? OrgType { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OrgUser, OrgUserDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.User! != null! ? s.User.Id : 0))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.User! != null! ? s.User.FirstName : ""))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.User! != null! ? s.User.LastName : ""))
                .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User! != null! ? s.User.FullName : ""))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.User! != null! ? s.User.Email : ""))
                .ForMember(d => d.MobilePhone, opt => opt.MapFrom(s => s.User! != null! ? s.User.MobilePhone : ""))
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title! != null! ? s.Title : ""))
                .ForMember(d => d.OrgRole, opt => opt.MapFrom(s => s.Role! != null! ? s.Role.Name : null))
                .ForMember(d => d.OrgRoleId, opt => opt.MapFrom(s => s.Role! != null! ? s.Role.Id : 0))
                .ForMember(d => d.LastLoginDateTimeUtc, opt => opt.MapFrom(s => s.User! != null! ? s.User.LastLoginDateTimeUtc : null))
                .ForMember(d => d.LastModifiedDateTimeUtc, opt => opt.MapFrom(s => s.ModifyDateTimeUtc))
                .ForMember(d => d.IsDisabled, opt => opt.MapFrom(s => s.IsDisabled ? s.IsDisabled : s.User!.IsDisabled))
                .ForMember(d => d.IsInvitationPending, opt => opt.MapFrom(s => s.IsInvited && !s.IsInviteProcessed))
                .ForMember(d => d.UserInviteSentDateTimeUtc, opt => opt.MapFrom(s => s.User! != null! ? s.User.CreateDateTimeUtc : null))
                .ForMember(d => d.OrgType, opt => opt.MapFrom(s => s.Org!.Type));
        }
    }
}

#pragma warning restore S1125
