using IdentityExploration.DTO;
using Microsoft.AspNetCore.Authorization;
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
        public readonly UserManager<Employee> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        private readonly Token _token;
        public readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<Employee> userManager, 
            RoleManager<IdentityRole> roleManager, IConfiguration configuration, Token token)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _token = token;
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
                new Claim(ClaimTypes.Name, user.Email)
                // new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach(var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = _token.GetToken(authClaims);

            return Ok(token);
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

            Employee user = new Employee
            { 
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                JobPosition = model.JobPosition,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await AddRole(model.Email, "user");
                return Ok("Account created.");
            }
            else
            {
                return BadRequest("Failed to create account.");
            }
        }

        // Adding roles
        [Authorize(Roles = "admin")]
        [HttpPost("/addrole/")]
        public async Task<IActionResult> AddRole(string email, string role)
        {
            Employee user = await _userManager.FindByEmailAsync(email);
            if (role == null || role.Length == 0)
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
            if (!UserRoles.Roles.Contains(role))
            {
                return NotFound("Role not found. Available roles: ");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                try
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Server error: " + ex);
                }
            }
            return Ok(await _userManager.AddToRoleAsync(user, role));
        }

        [Authorize(Roles = "admin")]
        [HttpPost("/droprole/")]
        public async Task<IActionResult> Rroprole(string email, string role)
        {
            Employee user = await _userManager.FindByEmailAsync(email);
            if (role == null || role.Length == 0)
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
            if (!await _userManager.IsInRoleAsync(user, role))
            {
                return NotFound("User "+email+" doesn't have the role "+role);
            }
            else
            {
                return Ok(await _userManager.RemoveFromRoleAsync(user, role));
            }
        }

    }
}
