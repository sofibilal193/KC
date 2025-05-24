using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using FluentValidation;
using FluentValidation.Results;
using KC.Application.Common;
using KC.Application.Common.Auth;
using KC.Application.Common.Identity;
using KC.Domain.Common.ApiServices;
using KC.Domain.Common.Constants;
using KC.Domain.Common.Exceptions;

namespace KC.Infrastructure.Common
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppSettings _settings;
        private bool _isDevTokenRequest;

        public ApiService(IHttpClientFactory clientFactory, IHttpContextAccessor context,
            IServiceProvider serviceProvider, IOptions<AppSettings> settingsOptions)
        {
            _clientFactory = clientFactory;
            _context = context;
            _serviceProvider = serviceProvider;
            _settings = settingsOptions.Value;
        }

        public async Task<TResponse?> GetAsync<TResponse>(string serviceType, string url, CancellationToken cancellationToken = default)
        {
            var client = await GetHttpClientAsync(serviceType, cancellationToken);
            var response = await client.GetAsync(url, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }
            await ValidateResponseAsync(response, cancellationToken);
            return await ReadContentAsync<TResponse>(response, cancellationToken);
        }

        public async Task<TResponse?> GetAsync<TResponse>(string serviceType, string url, Dictionary<string, object?> queryParameters, CancellationToken cancellationToken = default)
        {
            var client = await GetHttpClientAsync(serviceType, cancellationToken);
            var parameters = queryParameters.ToDictionary(p => p.Key, p => p.Value?.ToString());
            url = QueryHelpers.AddQueryString(url, parameters);
            var response = await client.GetAsync(url, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }
            await ValidateResponseAsync(response, cancellationToken);
            return await ReadContentAsync<TResponse>(response, cancellationToken);
        }

        public async Task PostAsync<TRequest>(
            string serviceType, string url, TRequest request, CancellationToken cancellationToken = default)
        {
            var client = await GetHttpClientAsync(serviceType, cancellationToken);
            var response = await client.PostAsJsonAsync(url, request, cancellationToken);
            await ValidateResponseAsync(response, cancellationToken);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string serviceType, string url, TRequest request, CancellationToken cancellationToken = default)
        {
            var client = await GetHttpClientAsync(serviceType, cancellationToken);
            var response = await client.PostAsJsonAsync(url, request, cancellationToken);
            await ValidateResponseAsync(response, cancellationToken);
            return await ReadContentAsync<TResponse>(response, cancellationToken);
        }

        public async Task<TResponse?> PostContentAsync<TResponse>(
          string serviceType, string url, MultipartFormDataContent content, CancellationToken cancellationToken = default)
        {
            var client = await GetHttpClientAsync(serviceType, cancellationToken);
            var response = await client.PostAsync(url, content, cancellationToken);
            await ValidateResponseAsync(response, cancellationToken);
            return await ReadContentAsync<TResponse>(response, cancellationToken);
        }

        public async Task PutAsync<TRequest>(
            string serviceType, string url, TRequest request, CancellationToken cancellationToken = default)
        {
            var client = await GetHttpClientAsync(serviceType, cancellationToken);
            var response = await client.PutAsJsonAsync(url, request, cancellationToken);
            await ValidateResponseAsync(response, cancellationToken);
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(
            string serviceType, string url, TRequest request, CancellationToken cancellationToken = default)
        {
            var client = await GetHttpClientAsync(serviceType, cancellationToken);
            var response = await client.PutAsJsonAsync(url, request, cancellationToken);
            await ValidateResponseAsync(response, cancellationToken);
            return await ReadContentAsync<TResponse>(response, cancellationToken);
        }

        public async Task DeleteAsync(string serviceType, string url, CancellationToken cancellationToken = default)
        {
            var client = await GetHttpClientAsync(serviceType, cancellationToken);
            var response = await client.DeleteAsync(url, cancellationToken);
            await ValidateResponseAsync(response, cancellationToken);
        }

        public async Task<TResponse?> DeleteAsync<TResponse>(
            string serviceType, string url, CancellationToken cancellationToken = default)
        {
            var client = await GetHttpClientAsync(serviceType, cancellationToken);
            var response = await client.DeleteAsync(url, cancellationToken);
            await ValidateResponseAsync(response, cancellationToken);
            return await ReadContentAsync<TResponse>(response, cancellationToken);
        }

        private async Task<HttpClient> GetHttpClientAsync(string serviceType, CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient(serviceType);
            client.DefaultRequestVersion = HttpVersion.Version20;
            client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

            // forward auth header
            client.DefaultRequestHeaders.Authorization = _context.HttpContext?.GetAuthenticationHeaderValue();

            // if app using client credentials, aquire auth token
            if (_settings.AuthSettings.Type == ApiAuthType.ClientCredentials)
            {
                if (!string.IsNullOrEmpty(_settings.AuthSettings.ClientCredentials?.ClientId))
                {
                    if (string.IsNullOrEmpty(_settings.AuthSettings.ClientCredentials?.Scope))
                    {
                        throw new DomainException("AppSettings.AuthSettings.ClientCredentials.Scope is required when Type is ClientCredentials.");
                    }
                    string[] scopes = { _settings.AuthSettings.ClientCredentials.Value.Scope! };
                    var clientApp = _serviceProvider.GetRequiredService<IConfidentialClientApplication>();
                    var result = await clientApp.AcquireTokenForClient(scopes).ExecuteAsync(cancellationToken);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                }
                else if (!_isDevTokenRequest)
                {
                    // get dev token
                    var command = new ValidateDevUserCommand(_settings.AuthSettings.BasicAuthUserName ?? "",
                        _settings.AuthSettings.BasicAuthPassword ?? "");
                    _isDevTokenRequest = true;
                    var response = await PostAsync<ValidateDevUserCommand, ValidateDevUserResponse>(
                        ApiServiceTypes.Identity, "devtokens", command, cancellationToken);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);
                    _isDevTokenRequest = false;
                }
            }

            // forward API key
            var apiKey = _context.HttpContext?.GetApiKey() ?? _settings.AuthSettings.ApiKey;
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Add(AuthConstants.ApiKeyHeaderName, apiKey);
            }

            // forward client ip address
            var clientIp = _context.HttpContext?.GetRequestIp();
            if (!string.IsNullOrEmpty(clientIp))
            {
                client.DefaultRequestHeaders.Add("X-Forwarded-For", clientIp);
            }

            // forward session-id
            var sessionId = _context.HttpContext?.GetHeaderValueAs<string>("Session-Id");
            if (sessionId is not null)
            {
                client.DefaultRequestHeaders.Add("Session-Id", sessionId);
            }

            return client;
        }

        private static async Task ValidateResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var problemDetails = await response.Content
                    .ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: cancellationToken);
                if (problemDetails is not null)
                {
                    var errors = new List<ValidationFailure>();
                    foreach (var error in problemDetails.Errors)
                    {
                        errors.AddRange(error.Value.Select(message => new ValidationFailure(error.Key, message)));
                    }
                    throw new ValidationException(errors);
                }
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new HttpRequestException("Forbidden", null, HttpStatusCode.Forbidden);
            }

            response.EnsureSuccessStatusCode();
        }

        private static async Task<T?> ReadContentAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)await response.Content.ReadAsStringAsync(cancellationToken);
            }
            else
            {
                return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
            }
        }
    }
}
