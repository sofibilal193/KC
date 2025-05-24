namespace KC.Domain.Common.Entities.IntegrationEvents
{
    public enum IntegrationEventState
    {
        NotPublished = 0,
        InProgress = 1,
        Published = 2,
        PublishFailed = 3
    }
}
