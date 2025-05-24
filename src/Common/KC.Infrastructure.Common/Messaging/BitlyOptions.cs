
namespace KC.Infrastructure.Common.Messaging
{
    public record BitlyOptions
    {
        public string ApiShortenerUri { get; init; } = string.Empty;

        public string AccessToken { get; init; } = string.Empty;

        public string Domain { get; init; } = string.Empty;

        public string? GroupGuid { get; init; }

        public BitlyOptions()
        { }

        public BitlyOptions(BitlyOptions options)
        {
            ApiShortenerUri = options.ApiShortenerUri;
            AccessToken = options.AccessToken;
            Domain = options.Domain;
            GroupGuid = options.GroupGuid;
        }
    }
}
