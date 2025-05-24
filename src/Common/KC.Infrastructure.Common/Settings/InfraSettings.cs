using KC.Infrastructure.Common.Cacheing;
using KC.Infrastructure.Common.RabbitMQ;
using KC.Infrastructure.Common.ServiceBus;
using KC.Infrastructure.Common.Storage;
using KC.Infrastructure.Common.Messaging;
using System;

namespace KC.Infrastructure.Common
{
    public record InfraSettings
    {
        #region Azure App Configuration Settings

        public string AzureAppConfigUri { get; init; } = "";

        public string AzureAppConfigSentinelKey { get; init; } = ""; // https://docs.microsoft.com/en-us/azure/azure-app-configuration/enable-dynamic-configuration-aspnet-core?tabs=core5x

        public TimeSpan AzureAppConfigCachePeriod { get; init; }

        #endregion

        #region Redis Cache Settings

        public RedisCacheOptions RedisCacheSettings { get; init; } = new();

        #endregion

        #region Azure Storage Settings

        public AzureStorageFileOptions AzureStorageFileSettings { get; init; } = new();

        #endregion

        #region RabbitMQ Settings

        public RabbitMQOptions RabbitMQSettings { get; init; } = new();

        #endregion

        #region Azure Service Bus Settings

        public ServiceBusOptions ServiceBusSettings { get; init; } = new();

        #endregion

        #region Azure SignalR Settings

        public AzureSignalRSettings SignalRSettings { get; init; } = new();

        #endregion

        #region Api Configurations

        public ApiServiceSettings ApiServiceSettings { get; set; } = new();

        #endregion

        #region Bitly Settings

        public BitlyOptions BitlySettings { get; set; } = new();

        #endregion

        #region Twilio Settings

        public TwilioClientOptions TwilioClientSettings { get; set; } = new();

        #endregion

        #region SendGrid Settings

        public string SendGridApiKey { get; set; } = string.Empty;

        #endregion

        #region AzureAD Settings

        public AzureADSettings AzureAD { get; init; } = new();

        #endregion

    }

    public record AzureADSettings
    {
        public string TenantId { get; init; } = string.Empty;

        public string ClientId { get; init; } = string.Empty;

        public string ClientSecret { get; init; } = string.Empty;

        public string B2CExtensionsAppId { get; init; } = string.Empty;


    }
}
