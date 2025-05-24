using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KC.Utils.Common;

namespace KC.Infrastructure.Common.Messaging
{
    public class BitlyUrlHttpClient : IBitlyUrlHttpClient
    {
        #region Private Variables

        private readonly ILogger<BitlyUrlHttpClient> _logger;
        private readonly HttpClient _client;
        private readonly BitlyOptions _options;

        #endregion

        public BitlyUrlHttpClient(ILogger<BitlyUrlHttpClient> logger,
            HttpClient client, BitlyOptions options)
        {
            _logger = logger;
            _options = options;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.AccessToken);
            _client = client;
        }

        #region Private Classes

        private sealed record ShortenRequest
        {
            [JsonPropertyName("group_guid")]
            public string? GroupGuid { get; init; }

            [JsonPropertyName("domain")]
            public string? Domain { get; init; }

            [JsonPropertyName("long_url")]
            public string? LongUrl { get; init; }
        }

        private sealed record ShortenResponse
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("link")]
            public string? Link { get; set; }
        }

        #endregion

        #region IBitlyUrlHttpClient Methods

        public async Task<string> GetShortUrlAsync(string longUrl,
            CancellationToken cancellationToken = default)
        {
            string shortUrl = longUrl;

            var request = new ShortenRequest
            {
                LongUrl = longUrl,
                Domain = _options.Domain,
                GroupGuid = _options.GroupGuid
            };

            try
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    _options.ApiShortenerUri)
                {
                    Content = request.ToJsonHttpContent()
                };

                using var httpResponse = await _client
                    .SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                httpResponse.EnsureSuccessStatusCode();
                using HttpContent responseContent = httpResponse.Content;
                var strResponse = await responseContent.ReadAsStringAsync(cancellationToken);
                var response = strResponse.TryParseTo<ShortenResponse>();
                if (response != null && !string.IsNullOrEmpty(response.Link))
                {
                    shortUrl = response.Link;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error has occurred while attempting to shorten URL via Bitly. BitlyApiShortenerUrl: {BitlyApiShortenerUrl}. BitlyApiAccessToken: {BitlyApiAccessToken}. LongUrl: {LongUrl}.", _options.ApiShortenerUri, _options.AccessToken, longUrl);
                shortUrl = longUrl;
            }

            return shortUrl;
        }

        #endregion

    }
}
