namespace KC.Infrastructure.Common.Cacheing
{
    public record RedisCacheOptions
    {
        public string ConnectionString { get; init; } = "";

        public int ReconnectMinIntervalSeconds { get; init; }

        public int ReconnectErrorThresholdSeconds { get; init; }

        public int RestartConnectionTimeoutSeconds { get; init; }

        public int RetryMaxAttempts { get; init; }

        public RedisCacheOptions() { }

        public RedisCacheOptions(RedisCacheOptions options)
        {
            ConnectionString = options.ConnectionString;
            ReconnectErrorThresholdSeconds = options.ReconnectErrorThresholdSeconds;
            ReconnectMinIntervalSeconds = options.ReconnectMinIntervalSeconds;
            RestartConnectionTimeoutSeconds = options.RestartConnectionTimeoutSeconds;
            RetryMaxAttempts = options.RetryMaxAttempts;
        }
    }
}
