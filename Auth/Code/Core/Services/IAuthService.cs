using System;
using System.Threading.Tasks;
using CoreAuth.Models;

namespace CoreAuth.Services
{
    public interface IAuthService
    {
        
        Task<User> GetUserAsync(int userId);

        Task<bool> ValidateEmailAsync(string email);

        Task<LoginValidation> ValidateLoginAsync(string email, string password);

        string GenerateToken(int numberOfBytes);

        Task<int> UpdateUserAsync(User user);

        Task InitPasswordRecoveryAsync(string email, string remoteIpAddress);

        Task<Recovery> CheckPasswordRecoveryAsync(string key, string email);

        Task<Recovery> PasswordRecoveryAsync(string email, string key, string newPassword);

        Task<Tuple<bool, string>> ChangePasswordAsync(string email, string currentPassword, string newPassword);
        
    }
}
