using System.ComponentModel.DataAnnotations;

namespace IdentityExploration.DTO
{
    // 11. Nothing so special to mention 
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
