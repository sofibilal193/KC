// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using KC.Application.Common.Controllers;
// using KC.Config.API.Application;

// namespace KC.Config.API.Controllers
// {
//     [ApiVersion("1.0")]
//     [AllowAnonymous]
//     [Route("api/v{version:apiVersion}/themes")]
//     public class ThemesController : BaseController
//     {
//         /// <summary>
//         /// This method gets default theme, logo and a list of themes.
//         /// </summary>
//         /// <param name="domain" type="System.String" from="route" required="true">domain name of the calling app e.g. app.domain.com.</param>
//         /// <param name="userId" type="System.Int" from="route" required="false">ID of the logged-in user in the ODL database.</param>
//         /// <param name="orgId" type="System.String" from="route" required="true">ID of the organization the user has logged-in under.</param>
//         /// <returns></returns>
//         [HttpGet("{domain}/{userId}/{orgId}")]
//         [ProducesResponseType(StatusCodes.Status200OK)]
//         [ProducesResponseType(StatusCodes.Status404NotFound)]
//         public async Task<ActionResult<ThemeDto>> GetAsync(string domain, int? userId, int? orgId)
//         {
//             var dto = await Mediator.Send(new GetThemesQuery
//             {
//                 Domain = domain,
//                 UserId = userId,
//                 OrgId = orgId
//             });
//             if (dto?.Themes?.Count > 0)
//                 return Ok(dto);
//             else
//                 return NotFound();
//         }
//     }
// }
