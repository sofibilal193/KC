using System;
using AutoMapper;
using KC.Identity.API.Entities;
using ProtoBuf;

namespace KC.Identity.API.Application
{
    [Serializable]
    [ProtoContract]
    public record PermissionUserDto
    {
        /// <summary>
        /// Id of user.
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; init; }

        /// <summary>
        /// Full Name of User.
        /// </summary>
        [ProtoMember(2)]
        public string FullName { get; init; } = string.Empty;
    }
}
