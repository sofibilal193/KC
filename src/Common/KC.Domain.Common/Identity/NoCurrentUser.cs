using KC.Domain.Common.Constants;

namespace KC.Domain.Common.Identity
{
    public class NoCurrentUser : ICurrentUser
    {
        public bool IsAuthenticated => false;

        public string AuthProvider => AuthConstants.AuthProvider;

        public string? AuthProviderId => null;

        public string? FullName => null;

        public string? FirstName => null;

        public string? LastName => null;

        public string? Email => null;

        public string? Source => null;

        public int? UserId => default;

        public string? MobilePhone => default;

        public bool IsDisabled => false;

        public bool IsMultipleOrgs => false;

        public OrgType OrgType => OrgType.Dealer;

        public List<int> OrgIds => new();
    }
}
