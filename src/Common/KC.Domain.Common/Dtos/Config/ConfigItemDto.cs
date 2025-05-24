using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public record ConfigItemDto
    {
        [ProtoMember(1)]
        public string Type { get; init; } = "";

        [ProtoMember(2)]
        public string Name { get; init; } = "";

        [ProtoMember(3)]
        public string? Value { get; init; }

        [ProtoMember(4)]
        public string? Description { get; init; }

        [ProtoMember(5)]
        public short DisplayOrder { get; init; } = 0;

        [ProtoMember(6)]
        public bool IsInternal { get; init; }

        [ProtoMember(7)]
        public bool IsEncrypted { get; init; }
    }
}
