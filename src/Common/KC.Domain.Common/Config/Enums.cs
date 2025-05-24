
namespace KC.Domain.Common.Config
{
    public enum ProviderType : byte
    {
        DMSCRM = 0,
        UnitData = 1,
        UnitValuation = 2,
        Credit = 3,
        Product = 4,
        ESign = 5
    }

    public enum FieldType : byte
    {
        Text,
        TextArea,
        NumericDecimal,
        NumericWhole,
        NumericCurrency,
        DropDown,
        DropDownMulti,
        Checkbox,
        Radio,
        Signature,
        Initial,
        DateTime,
        DateOnly,
        Json,
        File,
        Url,
        Integer,
        Currency
    }

    public enum FieldName
    {
        InterestRate,
        DaysToFirstPayment,
        Term,
        TermFrequency,
        MenuAcceptanceEmail,
        LenderList,
        PrivacyPolicy,
        TermsOfUse,
        LinkExpiration,
        Logo,
        ShowProductCost,
        ShowPayments,
        ItemizePurchasePrice
    }

    public enum FeeType : byte
    {
        Doc = 1,
        PublicOfficial = 2,
        Dealer = 3,
        Other = 99
    }
}
