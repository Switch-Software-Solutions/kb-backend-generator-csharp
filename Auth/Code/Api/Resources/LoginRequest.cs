using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace ApiAuth.Resources
{
    
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
