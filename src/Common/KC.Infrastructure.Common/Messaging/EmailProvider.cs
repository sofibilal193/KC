using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using KC.Application.Common.Messaging;
using KC.Domain.Common.Exceptions;
using KC.Domain.Common.Messaging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace KC.Infrastructure.Common.Messaging
{
    /// <summary>
    /// Email provider class that implements IEmailprovider
    /// </summary>
    public class EmailProvider : IEmailProvider
    {
        #region Private Properties

        private readonly ISendGridClient _client;

        #endregion

        #region Constructors

        public EmailProvider(ISendGridClient client)
        {
            _client = client;
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

        #region IEmailProvider Methods

        public async Task<bool> SendAsync(IList<EmailMessage> messages,
            CancellationToken cancellationToken = default)
        {
            var list = new List<string>();

            foreach (var msg in messages)
            {
                var message = new SendGridMessage()
                {
                    From = new SendGrid.Helpers.Mail.EmailAddress(msg.From.Address, msg.From.Name),
                    Subject = msg.Subject,
                    PlainTextContent = msg.IsHtml ? null : msg.Body,
                    HtmlContent = msg.IsHtml ? msg.Body : null,
                };

                // set replyto
                if (msg.ReplyTo.HasValue)
                {
                    message.ReplyTo = new SendGrid.Helpers.Mail.EmailAddress
                        (msg.ReplyTo.Value.Address, msg.ReplyTo.Value.Name);
                }

                // add To recipients
                if (msg.To != null)
                {
                    foreach (var item in msg.To)
                    {
                        if (!CheckEmailExists(list, item.Address))
                            message.AddTo(new SendGrid.Helpers.Mail.EmailAddress { Email = item.Address, Name = item.Name });
                    }
                }

                // add Cc recipients
                if (msg.Cc != null)
                {
                    foreach (var item in msg.Cc)
                    {
                        if (!CheckEmailExists(list, item.Address))
                            message.AddCc(new SendGrid.Helpers.Mail.EmailAddress { Email = item.Address, Name = item.Name });
                    }
                }

                // add Bcc recipients
                if (msg.Bcc != null)
                {
                    foreach (var item in msg.Bcc)
                    {
                        if (!CheckEmailExists(list, item.Address))
                            message.AddBcc(new SendGrid.Helpers.Mail.EmailAddress { Email = item.Address, Name = item.Name });
                    }
                }

                // set email template if needed
                if (!string.IsNullOrEmpty(msg.TemplateId))
                {
                    message.SetTemplateId(msg.TemplateId);
                }

                // set template data
                if (msg.TemplateData != null)
                {
                    message.SetTemplateData(msg.TemplateData);
                }

                // set unsubscribe id
                if (msg.UnSubscribeGroupId.HasValue)
                {
                    message.Asm = new ASM
                    {
                        GroupId = msg.UnSubscribeGroupId.Value,
                        GroupsToDisplay = new List<int> { msg.UnSubscribeGroupId.Value }
                    };
                }

                if (message.Personalizations?.Count > 0)
                {
                    var response = await _client.SendEmailAsync(message, cancellationToken);
                    if (response != null)
                    {
                        var code = response.StatusCode;
                        if (code != HttpStatusCode.OK && code != HttpStatusCode.Accepted)
                        {
                            var result = await response.Body.ReadAsStringAsync(cancellationToken);
                            throw new DomainException(result);
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #region Private Methods

        private static bool CheckEmailExists(List<string> list, string email)
        {
            bool exists = list.Contains(email.ToLowerInvariant());
            if (!exists)
                list.Add(email.ToLowerInvariant());
            return exists;
        }

        #endregion
    }
}
