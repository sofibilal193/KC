using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KC.Application.Common.Auth;
using KC.Application.Common.Controllers;

namespace KC.Identity.API.Controllers
{
    // A simple controller that issues JWT tokens in a DEV environment
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/devtokens")]
    [ApiVersion("1.0")]
    public class DevTokenController : BaseController
    {
        [HttpPost]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
        public async Task<ActionResult<ValidateDevUserResponse>> ValidateCredentialsAsync(ValidateDevUserCommand cmd)
        {
            var response = await Mediator.Send(cmd);
            if (response == default)
                return Unauthorized();
            else
                return Ok(response);
        }
    }
}
