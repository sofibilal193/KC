namespace KC.Infrastructure.Common.Messaging
{
    public record TwilioClientOptions
    {
        public string AccountSid { get; set; } = string.Empty;

        public string AuthToken { get; set; } = string.Empty;

        public TwilioClientOptions() { }

        public TwilioClientOptions(TwilioClientOptions options)
        {
            AccountSid = options.AccountSid;
            AuthToken = options.AuthToken;
        }
    }
}
