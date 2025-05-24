
namespace KC.Identity.API.Entities
{
    public readonly record struct DevUser(string Email, string Password,
        string FirstName, string LastName)
    {
        public string FullName => string.Concat(FirstName, " ", LastName);
    }
}
