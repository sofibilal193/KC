namespace KC.Domain.Common
{
    public record NotificationPayloadDto
    {
        public EntityUpdateType Action { get; set; }
        public object? Payload { get; set; }
    }
}
