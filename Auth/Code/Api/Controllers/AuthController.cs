using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ApiAuth.Resources;
using CoreAuth.Models;
using CoreAuth.Services;
using ApiAuth.Helpers;
using AutoMapper;

namespace ApiAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authentication;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IMapper _mapper;

        public AuthController(IConfiguration configuration, IAuthService authentication, IMapper mapper)
        {
            _configuration = configuration;
            _authentication = authentication;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            _mapper = mapper;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest user)
        {
            if (user == null)
            {
                return BadRequest("Invalid login request");
            }

            var validation = await _authentication.ValidateLoginAsync(user.Email, user.Password);

            if (validation.IsValid)
            {
                var User = validation.User;

                var tokenExpirationMinutes = int.Parse(_configuration.GetSection("Auth:TokenExpirationMinutes").Value);

                var accessTokenString = GenerateNewAccessToken(User);

                string refreshToken = "";
                if (!User.TryFindValidRefreshToken(out refreshToken))
                {
                    refreshToken = _authentication.GenerateToken(32);

                    // Add refresh token in user and persist
                    int refreshTokenExpirationDays = int.Parse(_configuration.GetSection("Auth:RefreshTokenExpirationDays").Value);

                    User.AddRefreshToken(refreshToken, validation.User.Id,
                        Request.HttpContext.Connection.RemoteIpAddress?.ToString(), refreshTokenExpirationDays);

                    await _authentication.UpdateUserAsync(User);
                }

                var loginResponse = new LoginResponse()
                {
                    AccessToken = new AccessToken
                    {
                        Token = accessTokenString,
                        ExpiresIn = (int)TimeSpan.FromMinutes(tokenExpirationMinutes).TotalSeconds
                    },
                    RefreshToken = refreshToken
                };
                return Ok(loginResponse);
            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("refreshtoken")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> ExchangeRefreshToken([FromBody] RefreshTokenRequest tokenInfo)
        {
            if (tokenInfo == null)
            {
                return BadRequest("Invalid token");
            }

            var signingKey = _configuration.GetSection("Auth:Key").Value;
            var tokenExpirationMinutes = int.Parse(_configuration.GetSection("Auth:TokenExpirationMinutes").Value);

            // Validate access token
            var cp = GetPrincipalFromToken(tokenInfo.AccessToken, signingKey);

            // Get user from token
            var userId = cp.GetUserId();
            if (userId == null)
            {
                return BadRequest("Invalid token");
            }
            // validate refresh token
            var user = await _authentication.GetUserAsync(userId.Value);
            if (user != null && user.HasRefreshToken(tokenInfo.RefreshToken))
            {
                // generate new token
                var accessTokenString = GenerateNewAccessToken(user);
                var newRefreshToken = _authentication.GenerateToken(32);

                // Add refresh token in user and persist
                user.AddRefreshToken(newRefreshToken, user.Id, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
                user.RemoveRefreshToken(tokenInfo.RefreshToken);
                await _authentication.UpdateUserAsync(user);

                var response = new LoginResponse()
                {
                    AccessToken = new AccessToken()
                    {
                        Token = accessTokenString,
                        ExpiresIn = (int)TimeSpan.FromMinutes(tokenExpirationMinutes).TotalSeconds
                    },
                    RefreshToken = newRefreshToken
                };
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("logout")]
        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Logout([FromBody] LogoutRequest tokenInfo)
        {
            if (tokenInfo == null)
            {
                return BadRequest("Invalid token");
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userId = identity.GetUserId();
                var user = await _authentication.GetUserAsync(userId.Value);

                if (user != null && user.HasRefreshToken(tokenInfo.RefreshToken))
                {
                    user.RemoveRefreshToken(tokenInfo.RefreshToken);

                    await _authentication.UpdateUserAsync(user);
                }
            }
            return Ok();
        }

        [Route("initrecovery")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> InitPasswordRecovery([FromBody][Required] InitRecoveryRequest initRecovery)
        {

            var validation = await _authentication.ValidateEmailAsync(initRecovery.Email);

            if (validation)
            {
                await _authentication.InitPasswordRecoveryAsync(initRecovery.Email, Request.HttpContext.Connection.RemoteIpAddress?.ToString());

                return Ok("ok");
            } else
            {
                return Unauthorized();
              
            }
            
        }

        [Route("checkrecovery")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<RecoveryResponse>> CheckTokenRecovery([FromBody][Required] CheckTokenRequest checkTokenReq)
        {
            var result = await _authentication.CheckPasswordRecoveryAsync(checkTokenReq.key, checkTokenReq.email);

            var response = _mapper.Map<Recovery, RecoveryResponse>(result);

            return Ok(response);
        }

        [Route("recovery")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<RecoveryResponse>> ChangePasswordUsingRecovery([FromBody][Required] RecoveryRequest recoveryData)
        {
            var result = await _authentication.PasswordRecoveryAsync(recoveryData.email, recoveryData.key, recoveryData.newPassword);

            var response = _mapper.Map<Recovery, RecoveryResponse>(result);

            if (response.IsValid)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [Route("changepassword")]
        [HttpPost]
        public async Task<ActionResult<RecoveryResponse>> ChangePassword([FromBody][Required] PasswordChangeRequest changeData)
        {
            var result = await _authentication.ChangePasswordAsync(changeData.Email, changeData.CurrentPassword, changeData.NewPassword);

            if (result.Item1)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        private string GenerateNewAccessToken(User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Auth:Key").Value));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            int tokenExpirationMinutes = int.Parse(_configuration.GetSection("Auth:TokenExpirationMinutes").Value);

            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration.GetSection("Auth:Issuer").Value,
                audience: _configuration.GetSection("Auth:Audience").Value,
                claims: CreateListOfClaims(user),
                expires: DateTime.Now.AddMinutes(tokenExpirationMinutes),
                signingCredentials: signingCredentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return accessToken;
        }

        private List<Claim> CreateListOfClaims(User user)
        {
            return new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(AuthorizationHelper.ClaimIdentifierUserId, user.Id.ToString())
            };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
        {
            return ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateLifetime = false // we check expired tokens here
            });
        }

        private ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            try
            {
                var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

