// https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/conventions?view=aspnetcore-6.0#create-web-api-conventions
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using KC.Application.Common.Validations;

namespace KC.Application.Common.Controllers
{
#pragma warning disable S1186 // Add a nested comment...
#pragma warning disable RCS1163 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
    //
    // Summary:
    //     Default api conventions.
    public static class ApiConventions
    {
        /// <summary>
        /// Create convention.
        /// </summary>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(401)]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ApiValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void Create([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] params object[] paramaters)
        { }

        /// <summary>
        /// Edit convention.
        /// </summary>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(401)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ApiValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void Edit([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] params object[] paramaters)
        { }

        /// <summary>
        /// Update convention.
        /// </summary>
        /// <param name="model"></param>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(401)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ApiValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void Update([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] params object[] paramaters)
        { }

        /// <summary>
        /// Post convention.
        /// </summary>
        /// <param name="model"></param>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ApiValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void Post([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object model)
        { }

        /// <summary>
        /// Put convention.
        /// </summary>
        /// <param name="model"></param>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(401)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ApiValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void Put([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object model)
        { }

        /// <summary>
        /// Delete convention.
        /// </summary>
        /// <param name="id"></param>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ApiValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void Delete([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object id)
        { }

        /// <summary>
        /// Get convention.
        /// </summary>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ApiValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void Get()
        { }

        /// <summary>
        /// Get convention.
        /// </summary>
        /// <param name="id"></param>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ApiValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void Get([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object id)
        { }

        /// <summary>
        /// Get convention.
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="id"></param>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ApiValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void Get([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] params object[] paramaters)
        { }

        /// <summary>
        /// Find convention.
        /// </summary>
        /// <param name="model"></param>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ApiValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public static void Find([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object model)
        { }
    }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RCS1163 // Remove unused parameter
#pragma warning restore S1186 // Add a nested comment...
}
