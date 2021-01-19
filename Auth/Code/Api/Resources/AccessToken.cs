using System;
namespace ApiAuth.Resources
{
    public class AccessToken
    {
        public string Token { get; set; }

        public int ExpiresIn { get; set; }
    }
}
