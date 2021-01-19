using System;
namespace CoreAuth.Models
{
    public class RecoveryKey
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public DateTime Expires { get; set; }

        public bool Active => DateTime.UtcNow <= Expires;

        public string RemoteIpAddress { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public DateTime Created { get; set; }
    }
}
