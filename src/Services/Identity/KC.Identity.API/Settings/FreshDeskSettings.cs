namespace KC.Identity.API
{
    public record FreshDeskSettings
    {
        public string? SharedSecretKey { get; init; }
    }
}
