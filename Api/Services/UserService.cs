using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Api.Entities;
using Api.Helpers;
using Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services
{
    public class UserService : IUserService
    {
        private List<User> _users = new List<User>
        {
            new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" },
            new User { Id = 2, FirstName = "Admin", LastName = "Super", Username = "admin", Password = "admin" }
        };

        private readonly AppSettings _appSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public UserService(IOptions<AppSettings> appSettings, TokenValidationParameters tokenValidationParameters)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null) return null;

            AuthenticateResponse result = new AuthenticateResponse();
            result.Token = this.GenerateToken(user);
            result.RefreshToken = this.GenerateRefreshToken(result.Token);

            return result;
        }

        public AuthenticateResponse RefreshToken(string token, string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principalRefresh = tokenHandler.ValidateToken(refreshToken, _tokenValidationParameters, out var securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid refresh token");

                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                if (!principalRefresh.Identity.Name.Equals(token)) return null;
                string username = principal.Identity.Name;
                var claim = principal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid));
                int id = int.Parse(claim == null ? "0" : claim.Value);
                User user = _users.FirstOrDefault(u => u.Username.Equals(username) && u.Id == id);

                AuthenticateResponse result = new AuthenticateResponse();
                result.Token = this.GenerateToken(user);
                result.RefreshToken = this.GenerateRefreshToken(result.Token);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GenerateToken(User user)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddSeconds(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        private string GenerateRefreshToken(string token)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, token)
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        public IEnumerable<User> GetAll()
        {
            return _users.Select(u => {
                u.Password = null;
                return u;
            });
        }
    }
}