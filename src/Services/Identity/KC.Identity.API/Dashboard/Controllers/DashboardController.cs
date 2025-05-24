using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KC.Application.Common.Controllers;
using KC.Domain.Common;
using KC.Identity.API.Dashboard.Application;

namespace KC.Identity.API.Dashboard.Controllers
{
    [Route("api/v{version:apiVersion}/dashboard")]
    [ApiVersion("1.0")]
    public class DashboardController : BaseController
    {
        ///<summary>
        /// Get orgs count for dashboard.
        /// </summary>
        [HttpGet("count/orgs")]
        public async Task<ActionResult<CountDto>> GetDashboardOrgsCountAsync()
        {
            var query = new GetDashboardOrgsCountQuery();
            var response = await Mediator.Send(query);
            return Ok(response);
        }

        ///<summary>
        /// Get users count for dashboard.
        /// </summary>
        [HttpGet("count/users")]
        public async Task<ActionResult<CountDto>> GetDashboardUsersCountAsync()
        {
            var query = new GetDashboardUsersCountQuery();
            var response = await Mediator.Send(query);
            return Ok(response);
        }
    }
}
