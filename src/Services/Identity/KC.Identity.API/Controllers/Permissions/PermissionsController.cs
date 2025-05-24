using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KC.Application.Common.Controllers;
using KC.Identity.API.Application;

namespace KC.Identity.API.Controllers
{
    [Route("api/v{version:apiVersion}/permissions")]
    [ApiVersion("1.0")]
    public class PermissionsController : BaseController
    {
        /// <summary>
        /// Gets a list of permissions.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PermissionDto>>> GetAllPermissionsAsync()
        {
            var dto = await Mediator.Send(new GetAllPermissionsQuery());
            return Ok(dto);
        }

        /// <summary>
        /// Gets a list of permissions for an org.
        /// </summary>
        /// <param name="orgId">ID of Org.</param>
        [HttpGet("~/api/v{version:apiVersion}/orgs/{orgId}/permissions")]
        public async Task<ActionResult<List<PermissionDto>>> GetOrgPermissionsAsync(int orgId)
        {
            var dto = await Mediator.Send(new GetOrgPermissionsQuery(orgId));
            return Ok(dto);
        }
    }
}
