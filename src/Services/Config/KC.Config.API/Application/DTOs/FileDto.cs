using ProtoBuf;

namespace KC.Config.API.Application
{
    [Serializable]
    [ProtoContract]
    public record FileDto
    {
        [ProtoMember(1)]
        public Stream? StreamValue { get; init; }

        [ProtoMember(2)]
        public string ContentType { get; init; } = "";
    }
}
