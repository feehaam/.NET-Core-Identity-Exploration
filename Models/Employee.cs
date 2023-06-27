using Microsoft.AspNetCore.Identity;

namespace IdentityExploration.Models
{
    public class Employee : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string JobPosition { get; set; } = string.Empty;
    }
}
