using System;
using System.Collections.Generic;
using ProtoBuf;
namespace KC.Identity.API.Application
{
    [Serializable]
    [ProtoContract]
    public record ImpersonateUserResponse
    {
        /// <summary>
        /// Title of user org.
        /// </summary>
        [ProtoMember(1)]
        public string Title { get; init; } = "";

        /// <summary>
        /// Role of user org.
        /// </summary>
        [ProtoMember(2)]
        public string Role { get; init; } = "";

        /// <summary>
        /// Permissions of user org.
        /// </summary>
        [ProtoMember(3)]
        public List<string>? Permissions { get; init; }
    }
}
