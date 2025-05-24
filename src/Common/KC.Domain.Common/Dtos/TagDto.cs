using System;
using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public record TagDto
    {
        [ProtoMember(1)]
        public string Name { get; init; } = "";

        [ProtoMember(2)]
        public string? Value { get; init; }
    }
}
