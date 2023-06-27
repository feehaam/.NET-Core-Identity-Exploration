using Microsoft.AspNetCore.Identity;

namespace IdentityExploration.Models
{
    // 11. This model is just a custom user model to add more fields in IdentityUser
    public class Employee : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string JobPosition { get; set; } = string.Empty;
    }
}
