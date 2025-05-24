
namespace KC.Config.API.Entities
{
    public record UserNotificationConfig
    {
        /// <summary>
        /// Category of notification configuration.
        /// </summary>
        public string Category { get; init; } = "";

        /// <summary>
        /// Description of notification configuration.
        /// </summary>
        public string? Description { get; init; }

        public List<UserNotification>? Notifications { get; init; }
    }

    public record UserNotification
    {
        /// <summary>
        /// Name of notification.
        /// </summary>
        public string Name { get; init; } = "";

        /// <summary>
        /// Description of notification.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Whether InApp notification is enabled.
        /// </summary>
        public bool InApp { get; init; }

        /// <summary>
        /// Roles to which notification will be sent.
        /// </summary>
        public List<string>? InAppRoles { get; init; }

        /// <summary>
        /// Whether Email notification is enabled.
        /// </summary>
        public bool Email { get; init; }

        /// <summary>
        /// Email address to which notification will be sent.
        /// </summary>
        public string? EmailAddress { get; init; }

        /// <summary>
        /// Whether SMS notification is enabled.
        /// </summary>
        public bool SMS { get; init; }

        /// <summary>
        /// Phone number to which notification will be sent.
        /// </summary>
        public string? SMSAddress { get; init; }

        /// <summary>
        /// Name of user who last modified the notification.
        /// </summary>
        public string? LastModifiedUserName { get; private set; }

        /// <summary>
        /// Last modified date of notification in UTC time.
        /// </summary>
        public DateTime? LastModifiedDateTimeUtc { get; private set; }

        public void SetUserDateTime(string? lastModifiedUserName, DateTime? lastModifiedDateTimeUtc)
        {
            LastModifiedUserName = lastModifiedUserName;
            LastModifiedDateTimeUtc = lastModifiedDateTimeUtc;
        }
    }
}
