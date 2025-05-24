using System.Collections.Generic;

namespace KC.Application.Common.Html
{
    public record PrintMenuData
    {
        public string? Logo { get; init; }
        public string? FullName { get; init; }
        public List<PrintUnitLable>? UnitLables { get; init; }
        public string? DealId { get; init; }
        public string? DeliveryDate { get; init; }
        public string? TotalPurchasePrice { get; init; }
        public string? TotalDiscount { get; init; }
        public string? FreightPrepHandling { get; init; }
        public string? PartsAccessoriesLabor { get; init; }
        public string? NetTradeIn { get; init; }
        public string? Taxes { get; init; }
        public string? Fees { get; init; }
        public string? DownPayment { get; init; }
        public string? Rebates { get; init; }
        public string? TotalDealAmount { get; init; }
        public PrintDealCustomerSignature? PrimarySignature { get; init; }
        public PrintDealCustomerSignature? SecondarySignature { get; init; }
        public bool? IsSelectedProducts { get; init; }
        public List<PrintDealProduct>? SelectedProducts { get; init; }
        public bool? IsNonSelectedProducts { get; init; }
        public List<PrintDealProduct>? NonSelectedProducts { get; init; }
        public PrintFinanceField? FinanceFields { get; init; }
    }

    public record PrintUnitLable
    {
        public string? UnitIndex { get; init; }
        public string? UnitDetail { get; init; }
        public string? VIN { get; init; }
        public string? StockNo { get; init; }
    }

    public record PrintDealCustomerSignature
    {
        public string? Signature { get; init; }
        public string? SignatureDate { get; init; }
        public string? SignatureName { get; init; }
    }

    public record PrintDealProduct
    {
        public string? UnitName { get; init; }
        public string? ProductName { get; init; }
        public string? ProviderName { get; init; }
        public string? Coverage { get; init; }
        public string? Term { get; init; }
        public string? TermType { get; init; }
        public string? CostPrice { get; init; }
        public string? Deductible { get; init; }
        public string? DeductibleType { get; init; }
    }

    public record PrintFinanceField
    {
        public string? Lender { get; init; }
        public string? InterestRate { get; init; }
        public string? Term { get; init; }
        public string? FirstPaymentDate { get; init; }
        public string? LastPaymentDate { get; init; }
        public string? BasePayment { get; init; }
        public string? FinalPayment { get; init; }
        public string? TotalAmountFinanced { get; init; }
        public string? Frequency { get; init; }
    }
}