using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KC.Application.Common.Controllers;
using KC.Config.API.Application;

using KC.Domain.Common;
using KC.Domain.Common.Constants;
using KC.Domain.Common.Dtos;
using KC.Domain.Common.Identity;


namespace KC.Config.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize(AuthPolicies.TokenOrApiKey)]
    [Route("api/v{version:apiVersion}/configs")]
    public class ConfigsController : BaseController
    {

    }
}
