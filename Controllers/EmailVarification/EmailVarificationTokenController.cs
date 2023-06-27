using IdentityExploration.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace IdentityExploration.Controllers.EmailVarification
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailVarificationTokenController : ControllerBase
    {
        private readonly UserManager<Employee> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmailVarificationTokenController(UserManager<Employee> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("/get_email_token")]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User doesn't exist.");
            }

            var alreadyVerified = await _userManager.IsEmailConfirmedAsync(user);
            if (alreadyVerified)
            {
                return BadRequest("Email is already verified.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{Request.Scheme}://{Request.Host}/confirm_email?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

            return Ok(confirmationLink.ToString());
        }

        [HttpGet("/confirm_email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Invalid email or token.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("user"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("user"));
                }
                await _userManager.AddToRoleAsync(user, "user");
                return Ok("Email confirmed successfully.");
            }
            else
            {
                return BadRequest("Failed to confirm email.");
            }
        }

        [HttpPost("/unverify")]
        public async Task<bool> Unverify(string email)
        {
            RequireEmailConfirmation uv = new RequireEmailConfirmation(_userManager);
            bool result = await uv.Require(email);
            return result;
        }
    }
}
