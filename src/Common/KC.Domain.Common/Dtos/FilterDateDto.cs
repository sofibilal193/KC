using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public record FilterDateDto
    {
        [ProtoMember(1)]
        public DateOnly DateRangeBegin { get; init; }

        [ProtoMember(2)]
        public DateOnly DateRangeEnd { get; init; }

        [ProtoMember(3)]
        public DateOnly LastDateRangeBegin { get; init; }

        [ProtoMember(4)]
        public DateOnly LastDateRangeEnd { get; init; }
    }
}
