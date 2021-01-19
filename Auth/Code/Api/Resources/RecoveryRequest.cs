using System;
namespace ApiAuth.Resources
{
    public class RecoveryRequest
    {
        public string email { get; set; }

        public string key { get; set; }

        public string newPassword { get; set; }
    }
}
