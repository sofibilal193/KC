using System.Text.RegularExpressions;

namespace KC.Application.Common.Extensions
{
    public static class FieldSanitizations
    {
        public static string? Sanitize(this string? s, string pattern, int? maxLength = null)
        {
            if (s is not null)
            {
                var result = new Regex(pattern, RegexOptions.NonBacktracking).Replace(s.TrimEnd(), string.Empty);
                if (maxLength.HasValue && s.Length > maxLength)
                {
                    result = s[..maxLength.Value];
                }
                return result;
            }
            return null;
        }

        public static string? Capitalize(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            else if (value.Length == 1)
            {
                return value.ToUpper();
            }
            return char.ToUpper(value[0]) + value[1..];
        }
    }
}
