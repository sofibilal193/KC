using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Http;
using HttpClient = Twilio.Http.HttpClient;

namespace KC.Infrastructure.Common.Messaging
{
    [ExcludeFromCodeCoverage]
    public class TwilioClient : ITwilioRestClient
    {
        private readonly TwilioRestClient _innerClient;

        public TwilioClient(TwilioClientOptions options, System.Net.Http.HttpClient httpClient)
        {
            // customize the underlying HttpClient
            httpClient.DefaultRequestHeaders.Add("X-Client-App", "ODL");
            _innerClient = new TwilioRestClient(
                options.AccountSid,
                options.AuthToken,
                httpClient: new SystemNetHttpClient(httpClient));
        }

        public Response Request(Request request) => _innerClient.Request(request);
        public Task<Response> RequestAsync(Request request) => _innerClient.RequestAsync(request);
        public string AccountSid => _innerClient.AccountSid;
        public string Region => _innerClient.Region;
        public HttpClient HttpClient => _innerClient.HttpClient;
    }
}
