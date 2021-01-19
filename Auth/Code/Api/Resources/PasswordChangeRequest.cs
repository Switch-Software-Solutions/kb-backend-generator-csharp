using System;
namespace ApiAuth.Resources
{
    public class PasswordChangeRequest
    {
        public string Email { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
