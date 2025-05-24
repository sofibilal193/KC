// https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/conventions?view=aspnetcore-6.0#create-web-api-conventions
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using KC.Application.Common.Validations;

namespace KC.Application.Common.Controllers
{
#nullable disable
#pragma warning disable S1186 // Add a nested comment...
#pragma warning disable RCS1163 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
    //
    // Summary:
    //     Default api conventions.
    public static class IdentityApiConventions
    {
        /// <summary>
        /// Accept convention.
        /// </summary>
        /// <param name="model"></param>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void Accept([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object model)
        { }

        /// <summary>
        /// Get convention (no auth).
        /// </summary>
        /// <param name="id"></param>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ApiValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void NoAuth_Get([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object id)
        { }
    }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RCS1163 // Remove unused parameter
#pragma warning restore S1186 // Add a nested comment...
#nullable restore
}
