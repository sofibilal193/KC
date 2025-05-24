using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace KC.Identity.API.Controllers.Auth
{
    [ApiVersion("1.0")]
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<int> GetUserInfo()
        {
            return 0;
        }
    }
}