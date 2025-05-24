namespace KC.Domain.Common.Application
{
    public record AddressCommand
    {
        public string Type { get; init; } = "";
        public string Address1 { get; init; } = "";
        public string? Address2 { get; init; }
        public string City { get; init; } = "";
        public string State { get; init; } = "";
        public string? County { get; init; }
        public string Country { get; init; } = "USA";
        public string ZipCode { get; init; } = "";
        public string? TimeZone { get; init; }
        public string? GooglePlaceId { get; init; }
    }
}
