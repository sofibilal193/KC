using System.Net;

namespace KC.Domain.Common.Extensions
{
    public static class StringExtensions
    {
        public static string? Encode(this string? data)
        {
            return WebUtility.UrlEncode(data);
        }
    }
}
