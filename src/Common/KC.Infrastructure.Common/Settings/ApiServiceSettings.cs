using System;
using System.Collections.Generic;

namespace KC.Infrastructure.Common
{
    public class ApiServiceSettings
    {
        public Dictionary<string, ApiSettings> Apis { get; set; } = new();

        public int DefaultRetryCount { get; set; }

        public int DefaultRetryDelayMs { get; set; }

        public Uri? GetBaseUri(string key)
        {
            var settings = Apis.GetValueOrDefault(key);
            if (settings is not null)
            {
                return new Uri(settings.BaseUrl);
            }
            return null;
        }

        public int GetRetryCount(string key)
        {
            var settings = Apis.GetValueOrDefault(key);
            return settings?.RetryCount ?? DefaultRetryCount;
        }

        public TimeSpan GetRetryDelay(string key)
        {
            var settings = Apis.GetValueOrDefault(key);
            return TimeSpan.FromMilliseconds(settings?.RetryDelayMs ?? DefaultRetryDelayMs);
        }

        public string? GetUserName(string key)
        {
            var settings = Apis.GetValueOrDefault(key);
            if (settings is not null)
            {
                return settings.UserName;
            }
            return null;
        }
    }

    public class ApiSettings
    {
        public string BaseUrl { get; set; } = "";

        public int? RetryCount { get; set; }

        public int? RetryDelayMs { get; set; }

        public string? UserName { get; set; }
    }
}
