using ProtoBuf;

namespace KC.Domain.Common
{
    [Serializable]
    [ProtoContract]
    public class MissingFieldDto
    {
        /// <summary>
        /// Type of attribute.
        /// </summary>
        [ProtoMember(1)]
        public string Type { get; init; } = "";

        /// <summary>
        /// Name of attribute.
        /// </summary>
        [ProtoMember(2)]
        public string Name { get; init; } = "";

        /// <summary>
        /// Description of attribute.
        /// </summary>
        [ProtoMember(3)]
        public string? Description { get; init; }

        /// <summary>
        /// Value of attribute.
        /// </summary>
        [ProtoMember(4)]
        public string Value { get; init; } = "";
    }
}
