namespace KC.Application.Common.HandleBars
{
    public interface IHandleBarProvider
    {
        public string GetHtmlReplacedPlaceHolders(string inputHtml, object data, string dataType);
    }
}
