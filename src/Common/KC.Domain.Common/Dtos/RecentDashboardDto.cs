namespace KC.Domain.Common
{
    [Serializable]
    public record RecentDashboardDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = "";

        public string LastUpdateUser { get; init; } = "";

        public DateTime LastUpdatedDateTimeUtc { get; init; }
    }
}
