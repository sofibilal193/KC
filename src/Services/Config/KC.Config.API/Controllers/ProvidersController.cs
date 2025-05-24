using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KC.Application.Common.Controllers;
using KC.Config.API.Application;
using KC.Domain.Common;
using KC.Domain.Common.Config;
using KC.Domain.Common.Constants;

namespace KC.Config.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/providers")]
    public class ProvidersController : BaseController
    { }
}
