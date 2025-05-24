using Microsoft.AspNetCore.Authorization;

namespace KC.Application.Common.Auth
{
    /// <summary>
    /// Authorization requirement that requires a user to belong to an
    /// org in order to access data for that org.
    /// </summary>
#pragma warning disable S2094 // Remove this empty class, write its code or make it an "interface".
    public class OrgUserRequirement : IAuthorizationRequirement
    {
    }
#pragma warning restore S2094 // Remove this empty class, write its code or make it an "interface".
}
