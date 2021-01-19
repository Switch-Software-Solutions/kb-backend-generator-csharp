using System;
namespace CoreAuth.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; private set; }

        public DateTime Expires { get; private set; }

        public int UserId { get; private set; }

        public bool Active => DateTime.UtcNow <= Expires;

        public string RemoteIpAddress { get; private set; }

        public DateTime Created { get; set; }

        public User User { get; set; }

        public RefreshToken(string token, DateTime expires, int userId, string remoteIpAddress)
        {
            Token = token;
            Expires = expires;
            UserId = userId;
            RemoteIpAddress = remoteIpAddress;
            Created = DateTime.UtcNow;
        }
    }
}
