using System.Collections.Generic;

namespace KC.Application.Common.Html
{
    public record PrintQuoteData
    {
        public string Logo { get; init; } = "";
        public string? FullName { get; init; }
        public PrintCustomerInfo? PrimaryCustomerInfo { get; init; }
        public PrintCustomerInfo? SecondaryCustomerInfo { get; init; }
        public string QuoteId { get; init; } = "";
        public string TotalAmount { get; init; } = "";
        public string? DownPayment { get; init; }
        public PrintEstimatedPayment? EstimatedPayment { get; init; }
        public PrintCustomerSignature? PrimarySignature { get; init; }
        public PrintCustomerSignature? SecondarySignature { get; init; }
        public PrintCustomerSignature? DeclinedCustomerInfo { get; init; }
        public List<PrintQuoteUnit> Units { get; init; } = new();
        public List<PrintQuoteTradeIn>? TradeIns { get; init; }
        public PrintUnInstalledPartsAccessory? UnInstalledPartsAccessories { get; init; }
        public bool Intent { get; init; }
    }

    public record PrintCustomerInfo
    {
        public string? CustomerName { get; init; }
        public string? CustomerAddress { get; init; }
        public string? CustomerPhone { get; init; }
        public string? CustomerEmail { get; init; }
    }

    public record PrintCustomerSignature
    {
        public string? Signature { get; init; }
        public string? SignatureDate { get; init; }
        public string? PrintSignatureDate { get; init; }
        public string? CustomerSignatureName { get; init; }
    }

    public record PrintQuoteUnit
    {
        public string? UnitIndex { get; init; } = "";
        public string Unit { get; init; } = "";
        public string? UnitSubTotal { get; init; }
        public string? VINName { get; init; }
        public string? VIN { get; init; }
        public string? Mileage { get; init; }
        public string PurchasePrice { get; init; } = "";
        public string? Discount { get; init; }
        public string? Labor { get; init; }
        public string? Rebates { get; init; }
        public string? EstimatedFees { get; init; }
        public string? EstimatedTaxes { get; init; }
        public string? PrepHandling { get; init; }
        public string? StockNo { get; init; }
        public string? Description { get; init; }
        public string? PartsTotal { get; init; }
        public PrintInstalledPartsAccessoryPart? InstalledPartsAccessoryParts { get; init; }
    }

    public record PrintInstalledPartsAccessoryPart
    {
        public string? InstalledParts { get; init; }
    }

    public record PrintQuoteTradeIn
    {
        public string TradeInIndex { get; init; } = "";
        public string? TradeIn { get; init; }
        public string? NetTradeIn { get; init; }
        public string? Lienholder { get; init; }
        public string? Payoff { get; init; }
        public string? Allowance { get; init; }
    }

    public record PrintUnInstalledPartsAccessory
    {
        public string? UnInstalledTotal { get; init; }
        public string? UninstalledPartsLabour { get; init; }
        public string? SalesOtherTax { get; init; }
        public string? UnInstalledParts { get; init; }
    }

    public record PrintEstimatedPayment
    {
        public string? Frequency { get; init; }
        public string? Term { get; init; }
        public string? Payment { get; init; }
    }
}
