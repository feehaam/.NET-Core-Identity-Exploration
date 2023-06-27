using IdentityExploration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace IdentityExploration.Controllers.EmailVarification
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailVarificationTokenController : ControllerBase
    {
        private readonly UserManager<Employee> _userManager;

        public EmailVarificationTokenController(UserManager<Employee> userManager)
        {
            _userManager = userManager;
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
                return Ok("Email confirmed successfully.");
            }
            else
            {
                return BadRequest("Failed to confirm email.");
            }
        }
    }
}
