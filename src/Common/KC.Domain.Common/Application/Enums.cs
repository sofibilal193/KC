namespace KC.Domain.Common
{
    public enum CustomerType
    {
        Individual,
        Joint,
        Business
    }

    public enum UnitAge
    {
        New,
        Used,
        CPO,
        Consignment,
        OnOrder
    }

    public enum EngineSizeType : byte
    {
        CC = 0,
        L = 1,
    }

    public enum EnginePowerType : byte
    {
        HP = 0,
        W = 1,
        KW = 2,
    }

    public enum UnitUse : byte
    {
        Personal = 0,
        Commercial = 1
    }

    public enum UnitOptionType : byte
    {
        OEM = 0,
        DealerPart = 1,
        DealerLabor = 2,
    }

    public enum UnitOptionSetupType : byte
    {
        SetupPrep = 1,
        DealerAdd = 2,
        CustomerAdd = 3,
        Warranty = 4
    }

    public enum DealType
    {
        Finance,
        Cash,
        Lease,
        Balloon
    }

    public enum CalendarType : byte
    {
        Weekly = 1,
        Monthly = 2,
        Annually = 3,
        Custom = 4
    }
    public enum EntityUpdateType
    {
        Added,
        Updated,
        Deleted,
    }

    public enum State
    {
        New = 1,
        Active = 2,
        InActive = 3,
        Blocked = 4,
        Closed = 5
    }

    public enum TaxProfileJurisdictionType : byte
    {
        All = 0,
        InState = 1,
        OutofState = 2
    }

    public enum TaxJurisdictionType : byte
    {
        Dealer = 0,
        Customer = 1,
        Common = 2
    }

    public enum TaxType : byte
    {
        SalesTaxAuto = 0,
        SalesTaxGeneral = 2,
        UseTaxAuto = 1,
        UseTaxGeneral = 3
    }

    public enum ProductRatingType : byte
    {
        Unit = 0,
        Deal = 1,
        CreditInsurance = 2,
        GAPAutoCalc = 3,
        GAPManual = 4
    }

    public enum DeductibleType : byte
    {
        Normal = 0,
        Disappearing = 1,
        Reducing = 2,
        Variable = 3
    }

    public enum DocStatus : byte
    {
        New = 0,
        Viewed = 1,
        Signed = 2,
        Downloaded = 3,
        Voided = 4,
        Remitted = 5,
        Cancelled = 6,
        Expired = 7,
        Archived = 9
    }

    public enum DocPageFieldActorType : byte
    {
        Dealer = 0,
        Buyer = 1,
        CoBuyer = 2,
        Guarantor = 3
    }

    public enum TaxCategory : byte
    {
        General = 0,
        Auto = 1,
        Medical = 2,
        Construction = 3
    }

    public enum PaymentMethod : byte
    {
        Cash = 1,
        Check = 2,
        CreditCard = 3,
        GiftCard = 4,
        OEMCoupon = 5,
        OEMCreditCard = 6,
        Paypal = 7,
        Other = 8,
    }

    public enum Stage
    {
        Deal,
        Quote
    }

    public enum SignatureEntityType
    {
        Quote,
        Menu
    }

    public enum SignatureActorType
    {
        Dealer,
        PrimaryCustomer,
        SecondaryCustomer
    }

    public enum CheckListStatus : byte
    {
        Incomplete = 0,
        Completed = 1
    }

    public enum DurationTermType : byte
    {
        Months = 0,
        Days = 1
    }

    public enum SubmissionStatus
    {
        Queued = 1,
        Submitted = 2,
        Completed = 3,
        Retry = 4,
        Failed = 5
    }
}
