using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using KC.Domain.Common.Identity;
using KC.Utils.Common;

namespace KC.Application.Common.Identity
{
    public static class ClaimsPrincipalExtensions
    {
        public static OrgType? GetOrgType(this ClaimsPrincipal user)
        {
            if (Enum.TryParse<OrgType>(user.FindFirst("extension_OrgType")?.Value, out var orgType))
            {
                return orgType;
            }
            return null;
        }

        public static bool IsSuperAdmin(this ClaimsPrincipal user)
        {
            if (bool.TryParse(user.FindFirst("extension_IsSuperAdmin")?.Value, out var value))
            {
                return value;
            }
            return false;
        }

        public static List<int> GetOrgIds(this ClaimsPrincipal user)
        {
            var orgIds = user.FindFirst("extension_OrgIds")?.Value;
            if (!string.IsNullOrEmpty(orgIds))
            {
                return orgIds.Split(',', StringSplitOptions.TrimEntries)
                    .ToList().ConvertAll(Convert.ToInt32);
            }
            return new();
        }

        public static bool IsMemberOfOrg(this ClaimsPrincipal user, int orgId)
        {
            return user.GetOrgIds().Contains(orgId);
        }

        public static int? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst("extension_UserId")?.Value?.ToNullableInt();
        }

        public static bool ContainsOrgIdClaim(this ClaimsPrincipal user)
        {
            return !string.IsNullOrEmpty(user.FindFirst("extension_OrgIds")?.Value);
        }

        public static bool ContainsUserIdClaim(this ClaimsPrincipal user)
        {
            return !string.IsNullOrEmpty(user.FindFirst("extension_UserId")?.Value);
        }
    }
}
