namespace KC.Persistence.Common.Settings
{
    public record SqlServerSettings
    {
        public int MaxRetryCount { get; init; } = 10;

        public int MaxRetryDelaySeconds { get; init; } = 30;
    }
}
