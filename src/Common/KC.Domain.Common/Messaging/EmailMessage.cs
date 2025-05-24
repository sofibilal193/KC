namespace KC.Domain.Common.Messaging
{
    public readonly record struct EmailMessage
    (
        EmailAddress From,
        List<EmailAddress> To,
        string Subject,
        string Body,
        bool IsHtml = false,
        EmailAddress? ReplyTo = default,
        List<EmailAddress>? Cc = default,
        List<EmailAddress>? Bcc = default,
        string? TemplateId = default,
        object? TemplateData = default,
        int? UnSubscribeGroupId = default
    );
}
