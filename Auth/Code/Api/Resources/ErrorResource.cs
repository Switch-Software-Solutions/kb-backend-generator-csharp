using System;
namespace ApiAuth.Resources
{
    public class ErrorResource
    {
        public int StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
