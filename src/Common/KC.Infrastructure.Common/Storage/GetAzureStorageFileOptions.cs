using Microsoft.Extensions.Options;

namespace KC.Infrastructure.Common.Storage
{
    public record GetAzureStorageFileOptions : AzureStorageFileOptions
    {
        public GetAzureStorageFileOptions(IOptionsMonitor<InfraSettings> settingsAccessor)
            : base(settingsAccessor.CurrentValue.AzureStorageFileSettings)
        {
        }
    }
}
