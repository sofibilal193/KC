
namespace KC.Domain.Common.AzureAD
{
    public readonly record struct AzureADUser(string Id, string FirstName,
        string LastName, string Email, string? MobilePhone, string? JobTitle)
    {
        public string FullName
        {
            get
            {
                return string.Concat(FirstName, " ", LastName).Trim();
            }
        }
    }
}
