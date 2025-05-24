using System;

namespace KC.Domain.Common.Constants
{
    public static class AuthPolicies
    {
        public const string ApiKey = "ApiKey";

        public const string Basic = "Basic";

        public const string OrgUser = "OrgUser";

        public const string TokenOrApiKey = "OrgUserOrApiKey";

        public const string SysAdmin = "Sys.Admin";
    }
}
