using Azure.Core;

namespace KC.Infrastructure.Common.Storage
{
    public record AzureStorageFileOptions
    {
        public string PrimaryUri { get; init; } = "";

        public string GeoRedundantSecondaryUri { get; init; } = "";

        public RetryOptions? RetryOptions { get; init; }

        public AzureStorageFileOptions() { }

        public AzureStorageFileOptions(AzureStorageFileOptions options)
        {
            PrimaryUri = options.PrimaryUri;
            GeoRedundantSecondaryUri = options.GeoRedundantSecondaryUri;
            RetryOptions = options.RetryOptions;
        }
    }
}
