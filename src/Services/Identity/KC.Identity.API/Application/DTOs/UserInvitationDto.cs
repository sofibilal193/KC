using System;
using AutoMapper;
using KC.Application.Common.Mappings;
using KC.Identity.API.Messaging;
using ProtoBuf;

namespace KC.Identity.API.Application.DTOs
{
    [Serializable]
    [ProtoContract]
    public class UserInvitationDto
    {
        /// <summary>
        /// Name of user that sent invitation.
        /// </summary>
        [ProtoMember(1)]
        public string InviteUserName { get; init; } = "";

        /// <summary>
        /// Email address of user that sent invitation.
        /// </summary>
        [ProtoMember(2)]
        public string InviteUserEmail { get; init; } = "";

        /// <summary>
        /// First name of invited user.
        /// </summary>
        [ProtoMember(3)]
        public string FullName { get; init; } = "";

        /// <summary>
        /// Id of organization that user is invited to.
        /// </summary>
        [ProtoMember(4)]
        public int OrgId { get; init; }

        /// <summary>
        /// Name of organization that user is invited to.
        /// </summary>
        [ProtoMember(5)]
        public string OrgName { get; init; } = "";

        /// <summary>
        /// Street address of organization.
        /// </summary>
        [ProtoMember(6)]
        public string OrgAddress { get; init; } = "";

        /// <summary>
        /// City of organization.
        /// </summary>
        [ProtoMember(7)]
        public string OrgCity { get; init; } = "";

        /// <summary>
        /// State of organization.
        /// </summary>
        [ProtoMember(8)]
        public string OrgState { get; init; } = "";

        /// <summary>
        /// Zip code of organization.
        /// </summary>
        [ProtoMember(9)]
        public string OrgZipCode { get; init; } = "";

        /// <summary>
        /// Phone number of organization
        /// </summary>
        [ProtoMember(10)]
        public string? OrgPhone { get; init; }
    }
}
