using IdentityExploration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityExploration.Controllers.ForgottenPassword
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgottenToken : ControllerBase
    {
        private readonly UserManager<Employee> _userManager;

        public ForgottenToken(UserManager<Employee> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("/forgotten_code")]
        public async Task<IActionResult> GetCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User doesn't exist");
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Ok(code);
        }

        [HttpPost("/change_password")]
        public async Task<IActionResult> ChangePassword(string email, string code, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User doesn't exist");
            }

            var result = await _userManager.ResetPasswordAsync(user, code, password);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully");
            }
            else
            {
                return BadRequest("Failed to change password");
            }
        }
    }
}
