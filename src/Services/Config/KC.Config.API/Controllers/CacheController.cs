using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KC.Application.Common.Controllers;
using KC.Config.API.Application;
using KC.Domain.Common.Constants;

namespace KC.Config.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize(AuthPolicies.ApiKey)]
    [Route("api/v{version:apiVersion}/cache")]
    public class CacheController : BaseController
    {
        /// <summary>
        /// Removes items from the cache based on a matching pattern
        /// </summary>
        /// <param name="command">The command containing one or more key patterns to match. * wildcards are supported.</param>
        // [HttpPut("reset")]
        // [ProducesResponseType(StatusCodes.Status204NoContent)]
        // public async Task<ActionResult> ResetAsync(ResetCacheCommand command)
        // {
        //     await Mediator.Send(command);
        //     return NoContent();
        // }
    }
}
