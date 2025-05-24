using ProtoBuf;

namespace KC.Domain.Common.Dtos
{
    [Serializable]
    [ProtoContract]
    public record ProductCategoryDto
    {
        /// <summary>
        /// Value of product category.
        /// </summary>
        [ProtoMember(1)]
        public string Value { get; init; } = "";

        /// <summary>
        /// Name of product category.
        /// </summary>
        [ProtoMember(2)]
        public string Name { get; init; } = "";

        /// <summary>
        /// Description of product category.
        /// </summary>
        [ProtoMember(3)]
        public string? Description { get; init; }

        /// <summary>
        /// Display order of product category.
        /// </summary>
        [ProtoMember(4)]
        public byte DisplayOrder { get; init; }
    }
}
