using MediatR;

namespace KC.Domain.Common.Notifications
{
    public abstract record EventLogNotification(string Event, DateTime DateTimeUtc,
        string? Source, string? Description, dynamic? UserId, dynamic? OrgId, dynamic? RecordId)
        : INotification;
}
