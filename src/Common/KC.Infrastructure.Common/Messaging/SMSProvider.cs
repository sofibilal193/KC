using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KC.Application.Common.Messaging;
using KC.Domain.Common.Messaging;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace KC.Infrastructure.Common.Messaging
{
    /// <summary>
    /// SMS provider class that implements ISmsProvider
    /// </summary>
    public class SmsProvider : ISmsProvider
    {
        #region Private Properties

        private readonly ITwilioRestClient _client;
        private readonly ILogger<SmsProvider> _logger;

        #endregion

        #region Constructors

        public SmsProvider(ITwilioRestClient client, ILogger<SmsProvider> logger)
        {
            _client = client;
            _logger = logger;
        }

        #endregion

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // dispose resources
        }

        #endregion

        #region ISmsProvider Methods

        public async Task SendAsync(IList<SmsMessage> messages,
            CancellationToken cancellationToken = default)
        {
            foreach (var msg in messages)
            {
                try
                {
                    var message = await MessageResource.CreateAsync(
                                to: new PhoneNumber(GetFormattedUSPhone(msg.To)),
                                from: new PhoneNumber(GetFormattedUSPhone(msg.From)),
                                body: msg.Body,
                                client: _client); // pass in the custom client
                    if (message != null && !string.IsNullOrEmpty(message.Sid))
                        msg.SetSid(message.Sid);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "SMS Message Failure. From: {from}. To: {to}.",
                        msg.From, msg.To);
                }
            }
        }

        #endregion

        #region Private Methods

        private static string GetFormattedUSPhone(string phone)
        {
            var util = PhoneNumbers.PhoneNumberUtil.GetInstance();
            var number = util.Parse(phone, "US");
            return util.Format(number, PhoneNumbers.PhoneNumberFormat.E164);
        }

        #endregion

    }
}
