﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityExploration.DTO
{
    // 12. Nothing so special to mention 
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public int Age { get; set; } = 0;
        [Required]
        public string JobPosition { get; set; } = string.Empty;
    }
}
