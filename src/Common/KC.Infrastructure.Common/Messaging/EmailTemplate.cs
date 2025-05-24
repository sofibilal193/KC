namespace KC.Infrastructure.Common.Messaging
{
    public class EmailTemplate
    {
        public string TemplateId { get; set; } = "";

        public string FromName { get; set; } = "";

        public string FromAddress { get; set; } = "";

        public int? UnSubscribeGroupId { get; set; }
    }
}
