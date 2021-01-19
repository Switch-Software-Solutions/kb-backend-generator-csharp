using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreAuth.Models
{
    public class User
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string Nick { get; set; }

        public string Email { get; private set; }

        public string Salt { get; set; }

        public string PasswordHash { get; set; }

        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        private readonly List<RecoveryKey> _recoveryKeys = new List<RecoveryKey>();
        public IReadOnlyCollection<RecoveryKey> RecoveryKeys => _recoveryKeys.AsReadOnly();

        public User() { /* Required by EF */ }

        public User(string nick, string email)
        {
            Nick = nick;
            Email = email;
        }

        public bool HasValidRefreshToken(string refreshToken)
        {
            return _refreshTokens.Any(rt => rt.Token == refreshToken && rt.Active);
        }

        public bool HasRefreshToken(string refreshToken)
        {
            return _refreshTokens.Any(rt => rt.Token == refreshToken);
        }

        public void AddRefreshToken(string token, int userId, string remoteIpAddress, double daysToExpire = 5)
        {
            _refreshTokens.Add(new RefreshToken(token, DateTime.UtcNow.AddDays(daysToExpire), userId, remoteIpAddress));
        }

        public void RemoveRefreshToken(string refreshToken)
        {
            _refreshTokens.Remove(_refreshTokens.First(t => t.Token == refreshToken));
        }

        /// <summary>
        /// Look for a token that is still valid
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool TryFindValidRefreshToken(out string token)
        {
            var tokenItem = _refreshTokens.Where(t => t.Expires > DateTime.UtcNow).OrderByDescending(t => t.Expires).FirstOrDefault();
            token = tokenItem?.Token;
            return (tokenItem != null) ? true : false;
        }

        public void AddRecoveryKey(string key, string remoteIpAddress, double minutesToExpire)
        {
            _recoveryKeys.Add(new RecoveryKey()
            {
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(minutesToExpire),
                UserId = this.Id,
                RemoteIpAddress = remoteIpAddress,
                Created = DateTime.UtcNow
            });
        }

        public void RemoveAllRecoveryKeys()
        {
            _recoveryKeys.RemoveAll(k => true);
        }

        public string GetFullName()
        {
            return this.Nick;
        }
    }
}
