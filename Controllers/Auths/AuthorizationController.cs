using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityExploration.Controllers.Auths
{
    // 18. Just some little demo controllers with Authorization limits to test authorization

    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        [HttpGet("/everyone")]
        public IActionResult F1()
        {
            return Ok("This is accessible by everyone.");
        }

        [Authorize]
        [HttpGet("/logged")]
        public IActionResult F2()
        {
            return Ok("This is accessible only by the logged users.");
        }

        [Authorize(Roles = "admin")]
        [HttpGet("/admin")]
        public IActionResult F3()
        {
            return Ok("This is accessible only by the adminstrative users.");
        }

        [Authorize(Roles = "admin,shop")]
        [HttpGet("/shop")]
        public IActionResult F5()
        {
            return Ok("This is accessible for both admin and shop.");
        }
    }
}
