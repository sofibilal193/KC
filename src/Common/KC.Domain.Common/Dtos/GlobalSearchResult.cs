namespace KC.Domain.Common
{
    /// <summary>
    /// Global search result
    /// </summary>
    /// <param name="Id">ID of record.</param>
    /// <param name="Type">Type of result.</param>
    /// <param name="PrimaryName">Primary customer or borrower name.</param>
    /// <param name="SecondaryName">Secondary customer or borrower name.</param>
    /// <param name="Email">Primary customer or borrower email.</param>
    /// <param name="Phone">Secondary customer or borrower phone.</param>
    public readonly record struct GlobalSearchResult(int Id, string Type, string? PrimaryName, DateTime? LastUpdatedUtc,
        string? SecondaryName = null, short? Year = null, string? Make = null, string? Model = null, string? Email = null, string? Phone = null);

    public static class GlobalSearchResultTypes
    {
        public const string Deal = "Deal";

        public const string Quote = "Quote";

        public const string CreditApplication = "CreditApplication";

        public const string Customer = "Customer";

        public const string Lead = "Lead";
    }
}
