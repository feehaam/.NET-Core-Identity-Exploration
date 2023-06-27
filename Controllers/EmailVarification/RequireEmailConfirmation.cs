using IdentityExploration.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityExploration.Controllers.EmailVarification
{
    public class RequireEmailConfirmation 
    {
        private readonly UserManager<Employee> _userManager;
        public RequireEmailConfirmation(UserManager<Employee> userManager)
        {
            _userManager = userManager;
        }

        async public Task<bool> Require(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                user.EmailConfirmed = false;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
