using System;
namespace CoreAuth.Models
{
    public class Recovery
    {
        public bool IsValid { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string ErrorMessage { get; set; }
    }
}
