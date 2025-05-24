namespace KC.Application.Common.Html
{
    public record PrintData
    {
        public PrintQuoteData? PrintQuoteData { get; set; }
        public PrintMenuData? PrintMenuData { get; set; }
    }
}
