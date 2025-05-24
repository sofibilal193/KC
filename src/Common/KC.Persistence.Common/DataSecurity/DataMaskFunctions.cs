namespace KC.Persistence.Common.DataSecurity
{
#pragma warning disable S3400 // Remove this method and declare a constant for this value.
    /// <summary>
    /// Data mask functions for dynamic data masking.
    /// For additional details see: https://docs.microsoft.com/en-us/sql/relational-databases/security/dynamic-data-masking?view=sql-server-ver15
    /// </summary>
    public static class DataMaskFunctions
    {
        public static string Default() => "default()";

        public static string Email() => "email()";

        public static string Random(int startRange, int endRange) => $"random({startRange}, {endRange})";

        public static string Partial(int prefix, string padding, int suffix) => $"partial({prefix}, \"{padding}\", {suffix})";

        public static string Datetime(string part) => $"datetime(\"{part}\")";
    }
#pragma warning restore S3400 // Remove this method and declare a constant for this value.
}
