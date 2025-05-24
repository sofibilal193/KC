using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KC.Application.Common.Controllers;
using KC.Identity.API.Application;
using KC.Identity.API.Application.Commands;

namespace KC.Identity.API.Controllers
{
    [Route("api/v{version:apiVersion}/orgs/{orgId}/roles")]
    [ApiVersion("1.0")]
    public class RolesController : BaseController
    {
        /// <summary>
        /// Gets a list of user roles for the application
        /// </summary>
        /// <remarks>Gets a list of user roles for the application</remarks>
        [HttpGet("~/api/v{version:apiVersion}/roles")]
        public async Task<ActionResult<IList<RoleDto>>> GetRolesAsync()
        {
            var dto = await Mediator.Send(new GetRolesQuery());
            if (dto?.Count > 0)
                return Ok(dto);
            else
                return Ok(new List<RoleDto>());
        }

        /// <summary>
        /// Adds a custom role with permissions
        /// </summary>
        /// <param name="orgId">ID of organization to add role with permsission.</param>
        /// <param name="command"></param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> CreateRolePermissionAsync(int orgId, UpsertRolePermissionCommand command)
        {
            command.SetIds(orgId);
            var roleId = await Mediator.Send(command);
            return Created("", roleId);
        }

        /// <summary>
        /// Updates a custom role with permissions
        /// </summary>
        /// <param name="orgId">ID of organization to update role with permissions.</param>
        /// <param name="roleId">ID of role.</param>
        /// <param name="command"></param>
        [HttpPut("{roleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateRolePermissionAsync(int orgId, int roleId, UpsertRolePermissionCommand command)
        {
            command.SetIds(orgId, roleId);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Gets a list of roles with type standard or custom.
        /// </summary>
        /// <param name="orgId">ID of organization to get roles.</param>
        /// <remarks>Gets a list of roles.</remarks>
        [HttpGet]
        public async Task<ActionResult<List<OrgRoleDto>>> GetOrgRolesAsync(int orgId)
        {
            var dto = await Mediator.Send(new GetOrgRolesQuery(orgId));
            return Ok(dto);
        }
    }
}
