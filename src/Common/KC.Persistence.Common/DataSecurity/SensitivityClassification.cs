namespace KC.Persistence.Common.DataSecurity
{
    public record SensitivityClassification(string Label, string InformationType, SensitivityRank Rank);

    public static class SensitivityLabels
    {
        public const string Public = "Public";
        public const string General = "General";
        public const string Confidential = "Confidential";
        public const string ConfidentialGdpr = "Confidential - GDPR";
        public const string HighlyConfidential = "Highly Confidential";
        public const string HighlyConfidentialGdpr = "Highly Confidential - GDPR";
    }

    public static class SensitivityInformationTypes
    {
        public const string Banking = "Banking";
        public const string ContactInfo = "Contact Info";
        public const string Credentials = "Credentials";
        public const string CreditCard = "Credit Card";
        public const string DateOfBirth = "Date of Birth";
        public const string Financial = "Financial";
        public const string Health = "Health";
        public const string Name = "Name";
        public const string NationalId = "National ID";
        public const string Networking = "Networking";
        public const string Ssn = "SSN";
        public const string Other = "Other";
    }

    public enum SensitivityRank
    {
        NONE = 0,
        LOW = 10,
        MEDIUM = 20,
        HIGH = 30,
        CRITICAL = 40
    }
}
