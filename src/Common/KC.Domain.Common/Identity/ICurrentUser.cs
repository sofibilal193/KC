namespace KC.Domain.Common.Identity
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }

        string AuthProvider { get; }

        string? AuthProviderId { get; }

        string? FullName { get; }

        string? FirstName { get; }

        string? LastName { get; }

        string? Email { get; }

        string? MobilePhone { get; }

        string? Source { get; }

        int? UserId { get; }

        bool IsDisabled { get; }

        bool IsMultipleOrgs { get; }

        OrgType OrgType { get; }

        List<int> OrgIds { get; }
    }
}
