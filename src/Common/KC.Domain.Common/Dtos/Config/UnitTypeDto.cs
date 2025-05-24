using ProtoBuf;

namespace KC.Domain.Common.Dtos
{
    [Serializable]
    [ProtoContract]
    public record UnitTypeDto
    {
        /// <summary>
        /// The name of the Type.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; init; } = "";

        /// <summary>
        /// The description of the Type.
        /// </summary>
        [ProtoMember(2)]
        public string? Description { get; init; }

        /// <summary>
        /// The list of <see cref="UnitSubTypeDto">Sub-Types</see>.
        /// </summary>
        [ProtoMember(3)]
        public IList<UnitSubTypeDto>? SubTypes { get; init; }
    }

    [Serializable]
    [ProtoContract]
    public record UnitSubTypeDto
    {
        /// <summary>
        /// The name of the Sub-Type.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; init; } = "";

        /// <summary>
        /// If the Sub-Type should be visible or not.
        /// </summary>
        [ProtoMember(2)]
        public bool IsMainUnit { get; init; }

        /// <summary>
        /// The description of the Sub-Type.
        /// </summary>
        [ProtoMember(3)]
        public string? Description { get; init; }

        /// <summary>
        /// The list of <see cref="UnitCategoryDto">Categories</see>.
        /// </summary>
        [ProtoMember(4)]
        public IList<UnitCategoryDto>? Categories { get; init; }

        /// <summary>
        /// The list of <see cref="UnitFieldDto">Fields</see>.
        /// </summary>
        [ProtoMember(5)]
        public IList<UnitFieldDto>? Fields { get; init; }
    }

    [Serializable]
    [ProtoContract]
    public record UnitCategoryDto
    {
        /// <summary>
        /// The name of the Category.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; init; } = "";

        /// <summary>
        /// The description of the Category.
        /// </summary>
        [ProtoMember(2)]
        public string? Description { get; init; }
    }

    [Serializable]
    [ProtoContract]
    public record UnitFieldDto
    {
        /// <summary>
        /// The name of the Field.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; init; } = "";

        /// <summary>
        /// The label to show for the Field.
        /// </summary>
        [ProtoMember(2)]
        public string Label { get; init; } = "";

        /// <summary>
        /// If the Field should be visible or not.
        /// </summary>
        [ProtoMember(3)]
        public bool IsVisible { get; init; }

        /// <summary>
        /// If the Field is required or not.
        /// </summary>
        [ProtoMember(4)]
        public bool IsRequired { get; init; }

        /// <summary>
        /// If the Field is required for rating or not.
        /// </summary>
        [ProtoMember(5)]
        public bool IsRatingRequired { get; init; }

        /// <summary>
        /// The regex to use for validating format.
        /// </summary>
        [ProtoMember(6)]
        public string? Regex { get; init; }

        /// <summary>
        /// The minimum length for the field.
        /// </summary>
        [ProtoMember(7)]
        public int? MinLength { get; init; }

        /// <summary>
        /// The maximum length for the field.
        /// </summary>
        [ProtoMember(8)]
        public int? MaxLength { get; init; }

        /// <summary>
        /// The default value for the field.
        /// </summary>
        [ProtoMember(9)]
        public string? DefaultValue { get; init; }

        /// <summary>
        /// If the Field is required for contracting or not.
        /// </summary>
        [ProtoMember(10)]
        public bool IsContractingRequired { get; init; }

        /// <summary>
        /// Determines if the field is required based on the age of the unit.
        /// </summary>
        [ProtoMember(11)]
        public List<string>? AgeRequired { get; init; }
    }
}
