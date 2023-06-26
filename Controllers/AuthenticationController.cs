using IdentityExploration.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityExploration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public readonly UserManager<IdentityUser> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // Login with JWT

        [HttpPost("/signin")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if(user == null)
            {
                return NotFound("User doesn't exist.");
            }
            if(await _userManager.CheckPasswordAsync(user, model.Password) == false)
            {
                return Unauthorized("Wrong password");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach(var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = GetToken(authClaims);

            return Ok(token);
        }

        private string GetToken(List<Claim> claims)
        {
            // Define the security key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THISISAVERYHARDSECURITYKEYNOBODYCANBREAK"));

            // Create signing credentials using the security key
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create a new JWT token with the provided claims and signing credentials
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expiration time
                signingCredentials: signingCredentials
            );

            // Generate the token string
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }



        // Registration
        [HttpPost("/signup/")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid properties for user");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return BadRequest("User with same email already exist.");
            }

            IdentityUser user = new IdentityUser { 
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok("Account created.");
            }
            else
            {
                return BadRequest("Failed to create account.");
            }
        }

        // Adding roles
        [HttpPost("/addrole/")]
        public async Task<IActionResult> AddRole(string email, string role)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(email);
            if(role == null || role.Length == 0)
            {
                return BadRequest("Invalid role");
            }
            else
            {
                role = role.ToLower();
            }
            if (user == null)
            {
                return NotFound("User not found");
            }
            if(!UserRoles.Roles.Contains(role))
            {
                return NotFound("Role not found. Available roles: ");
            }

            if(! await _roleManager.RoleExistsAsync(role))
            {
                try
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Server error: "+ex);
                }
            }
            return Ok(await _userManager.AddToRoleAsync(user, role));
        }

    }
}
