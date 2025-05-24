namespace KC.Config.API.Entities
{
    public record FieldValue
    {
        public string Value { get; init; } = "";

        public string? Text { get; init; }

        public string? Description { get; init; }

        public short DisplayOrder { get; init; }
    }
}
