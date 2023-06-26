using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityExploration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [HttpGet("/authorize/public/")]
        public IActionResult F1()
        {
            return Ok("This is an unauthorized public controller.");
        }

        [Authorize]
        [HttpGet("/authorize/logged/")]
        public IActionResult F2()
        {
            return Ok("This is an authorized controller, only logged in users can access");
        }

        [Authorize(Roles = "admin")]
        [HttpGet("/authorize/admin/")]
        public IActionResult F3()
        {
            return Ok("Only admins can see it.");
        }

        [Authorize(Roles = "user")]
        [HttpGet("/authorize/user/")]
        public IActionResult F4()
        {
            return Ok("Only users can see it.");
        }

        [Authorize(Roles = "shop")]
        [HttpGet("/authorize/shop/")]
        public IActionResult F5()
        {
            return Ok("Only shops can see it.");
        }

        [Authorize(Roles = "delivery")]
        [HttpGet("/authorize/delivery/")]
        public IActionResult F6()
        {
            return Ok("Only delivery man can see it.");
        }

        [Authorize(Roles = "shop")]
        [Authorize(Roles = "delivery")]
        [HttpGet("/authorize/shopanddelivery/")]
        public IActionResult F7()
        {
            return Ok("Shop and delivery man can see it.");
        }


        [Authorize(Roles = "admin")]
        [Authorize(Roles = "shop")]
        [HttpGet("/authorize/adminandshop/")]
        public IActionResult F8()
        {
            return Ok("Shop and admin man can see it.");
        }
    }
}
