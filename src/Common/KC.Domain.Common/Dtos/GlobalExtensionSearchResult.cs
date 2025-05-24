namespace KC.Domain.Common
{
    /// <summary>
    /// Global extension search result.
    /// </summary>
    /// <param name="Id">ID of record.</param>
    /// <param name="Type">Type of result.</param>
    /// <param name="PrimaryName">Primary customer.</param>
    /// <param name="SecondaryName">Secondary customer.</param>

    public readonly record struct GlobalExtensionSearchResult(int Id, string Type, string? PrimaryName, DateTime? LastUpdatedDateTimeUtc,
        string? SecondaryName = null, short? Year = null, string? Make = null, string? Model = null,
        string? VIN = null, string? StockNo = null, decimal? Amount = null, int? AssociateId = null);
}
