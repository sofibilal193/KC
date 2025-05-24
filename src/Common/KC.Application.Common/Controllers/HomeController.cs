#pragma warning disable CA1822

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace KC.Application.Common.Controllers
{
    [OpenApiIgnore]
    [AllowAnonymous]
    [Route("")]
    [Route("Home")]
    public class HomeController : BaseController
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
