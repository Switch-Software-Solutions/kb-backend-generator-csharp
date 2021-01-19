using System;
namespace ApiAuth.Resources
{
    public class LoginResponse
    {

        public AccessToken AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
