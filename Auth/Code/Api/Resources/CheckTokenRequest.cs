using System;
namespace ApiAuth.Resources
{
    public class CheckTokenRequest
    {
        public string key { get; set; }

        public string email { get; set; }
    }
}
