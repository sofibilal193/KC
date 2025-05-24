using System.Collections.Generic;
using KC.Infrastructure.Common.Messaging;

namespace KC.Identity.API
{
    public class ApiSettings
    {
        public Dictionary<string, EmailTemplate> EmailTemplates { get; set; } = new();
        public Dictionary<string, SmsTemplate> SmsTemplates { get; set; } = new();
    }
}
