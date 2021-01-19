using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CoreAuth;
using CoreAuth.Services;
using CoreAuth.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ServicesAuth
{
    public class AuthService : IAuthService
    {
        const string TOKENALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-.";

        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IEmailService _emailService;

        public AuthService(IUnitOfWork unitOfWork, /*IEmailService emailService,*/ IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            //_emailService = emailService;
            _configuration = configuration;
        }
        /// <summary>
        /// Generate a random string token
        /// </summary>
        /// <returns></returns>
        public string GenerateToken(int numberOfBytes = 32)
        {
            var randomNumber = new byte[numberOfBytes];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<int> UpdateUserAsync(User user)
        {
            _unitOfWork.UserRepository.Update(user);

            return await _unitOfWork.CommitAsync();
        }

        public async Task<bool> ValidateEmailAsync(string email)
        {
            var result = false;

            var user = await _unitOfWork.UserRepository.FindCompleteAsync(
                u => u.Email.Equals(email));

            if (user != null )
            {
                result = true;
            }

            return result;
        }

        public async Task<LoginValidation> ValidateLoginAsync(string email, string password)
        {
            var result = new LoginValidation { IsValid = false };

            var user = await _unitOfWork.UserRepository.FindCompleteAsync(
                u => u.Email.Equals(email));

            if (user != null && ValidatePassword(user, password))
            {

                result.IsValid = true;
                result.User = user;
            }

            return result;
        }

        public async Task<User> GetUserAsync(int userId)
        {
            return await _unitOfWork.UserRepository.GetByIdCompleteAsync(userId);
        }

        public string EncryptPassword(string password, string saltString)
        {
            var salt = new byte[16];
            salt = Convert.FromBase64String(saltString);

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        public string EncryptNewPassword(string password, out string saltString)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            saltString = Convert.ToBase64String(salt);

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        private bool ValidatePassword(User user, string password)
        {
            var encryptedPassword = EncryptPassword(password, user.Salt);

            return encryptedPassword.Equals(user.PasswordHash);
        }

        public async Task InitPasswordRecoveryAsync(string email, string remoteIpAddress)
        {
            var user = await _unitOfWork.UserRepository.FindCompleteAsync(u => u.Email == email);
            if (user != null)
            {
                // Crear y guardar clave de recupero
                var recoveryKey = GenerateRndToken(6);
                var keyExpirationMinutes = int.Parse(_configuration.GetSection("Auth:PasswordRecoveryExpirationMinutes").Value);

                user.RemoveAllRecoveryKeys();
                user.AddRecoveryKey(recoveryKey, remoteIpAddress, keyExpirationMinutes);
                _unitOfWork.UserRepository.Update(user);

                await _unitOfWork.CommitAsync();
                // Enviar correo
                SendRecoveryEmail(user, recoveryKey);
            }
            else
            {
                // Si el email no es usuario del sistema, se envía un email indicando que no existe.
                // No se notifica al cliente, para evitar que se utilice esta funcionalidad para descubrir cuentas válidas.
                SendRecoveryFailedEmail(email);
            }
        }
        public async Task<Recovery> CheckPasswordRecoveryAsync(string key, string email)
        {
            var result = new Recovery
            {
                IsValid = false
            };

            var user = await _unitOfWork.UserRepository.FindCompleteAsync(u => u.RecoveryKeys
                .Any(rk => rk.Key == key) && u.Email.Equals(email));

            if (user != null && user.RecoveryKeys.Any(rk => rk.Key == key && rk.Active))
            {
                result.IsValid = true;
                result.Email = user.Email;
                result.Name = user.GetFullName();
            }
            else
            {
                result.ErrorMessage = "El token ingresado no es válido, o ha expirado.";
            }

            return result;
        }

        public async Task<Recovery> PasswordRecoveryAsync(string email, string key, string newPassword)
        {
            var result = new Recovery
            {
                IsValid = false
            };

            var user = await _unitOfWork.UserRepository.FindCompleteAsync(u =>
                u.Email.Equals(email));
                

            if (user != null && user.RecoveryKeys.Any(rk => rk.Active && rk.Key.Equals(key)))
            {
                if (ValidateNewPassword(user.Email, newPassword))
                {
                    await ChangeUserPassword(user, newPassword);
                    result.IsValid = true;
                    result.Email = user.Email;
                    result.Name = user.GetFullName();
                }
                else
                {
                    result.ErrorMessage = "La contraseña no cumple los requisitos de seguridad.";
                }
            }
            else
            {
                result.ErrorMessage = "La clave de recuperación no es válida, o ha expirado.";
            }

            return result;
        }

        public async Task<Tuple<bool, string>> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var user = await _unitOfWork.UserRepository.FindCompleteAsync(u => u.Email == email);

            if (user != null && ValidatePassword(user, currentPassword))
            {
                if (ValidateNewPassword(user.Email, newPassword))
                {
                    await ChangeUserPassword(user, newPassword);
                    return new Tuple<bool, string>(true, "");
                }
                else
                {
                    return new Tuple<bool, string>(false, "La nueva contraseña no cumple los requisitos de seguridad.");
                }
            }
            else
            {
                return new Tuple<bool, string>(false, "Los datos del usuario no son válidos.");
            }
        }

        private async void SendRecoveryEmail(User user, string recoveryKey)
        {
            var mailAccountName = _configuration.GetSection("EmailConfiguration:AccountName").Value;
            var fromName = _configuration.GetSection("EmailConfiguration:FromName").Value;
            var recoveryLink = ComposeRecoveryUrl(recoveryKey);
            var emailContent = Resources.RecoveryEmail;

            emailContent = emailContent.Replace("#usernames", user.GetFullName());
            emailContent = emailContent.Replace("#accountaddress", user.Email);
            emailContent = emailContent.Replace("#recoverylink", recoveryLink);

            /*
            var message = new EmailMessage(mailAccountName,
                new EmailAddress
                {
                    Name = user.GetFullName(),
                    Address = user.Email
                }
            );
            message.FromName = fromName;
            message.Subject = "Recuperación de contraseña";
            message.Content = emailContent;

            await _emailService.Send(message);

            */
        }
        /// <summary>
        /// Envía correcto a una cuenta ingresada por el usuario, pero que no es válida en el sistema.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="recoveryKey"></param>
        private async void SendRecoveryFailedEmail(string emailAddress)
        {
            // Esta modalidad de notificación impide que se use el recupero de contraseña para tratar de identificar una cuenta de email válida.
            var mailAccountName = _configuration.GetSection("EmailConfiguration:AccountName").Value;
            var emailContent = Resources.RecoveryFailedEmail;

            emailContent = emailContent.Replace("#accountaddress", emailAddress);

            /*
            var message = new EmailMessage(mailAccountName,
                new EmailAddress
                {
                    Name = emailAddress,
                    Address = emailAddress
                }
            );
            message.FromName = _configuration.GetSection("EmailConfiguration:FromName").Value;
            message.Subject = "Error en Recuperación de contraseña";
            message.Content = emailContent;
           

            await _emailService.Send(message);

             */
        }

        private string ComposeRecoveryUrl(string key)
        {
            var baseUrl = _configuration.GetSection("Auth:WebSiteUrl").Value;
            var recoveryUrl = _configuration.GetSection("Auth:RecoveryUrl").Value;

            return baseUrl + recoveryUrl + key;
        }

        /// <summary>
        /// Generate random digit token
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private string GenerateRndToken(int length)
        {
            Random generator = new Random();

            return generator.Next(0, 999999).ToString("D" + length.ToString());
        }

        /// <summary>
        /// Validar que una contraseña respeta las reglas de formación
        /// </summary>
        /// <param name="password"></param>
        private bool ValidateNewPassword(string email, string password)
        {
            var minChars = int.Parse(_configuration.GetSection("Auth:PasswordMinChars").Value);

            if (!email.Contains(password) && password.Length >= minChars)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<int> ChangeUserPassword(User user, string newPassword)
        {
            string salt;
            var encryptedPassword = EncryptNewPassword(newPassword, out salt);

            user.PasswordHash = encryptedPassword;
            user.Salt = salt;

            return await UpdateUserAsync(user);
        }

        
    }
}
