using System;
namespace ApiAuth.Resources
{
    public class RecoveryResponse
    {
        public bool IsValid { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string ErrorMessage { get; set; }
    }
}
