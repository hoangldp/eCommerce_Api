using System.Collections.Generic;
using Api.Entities;
using Api.Models;

namespace Api.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(string username, string password);
        AuthenticateResponse RefreshToken(string token, string refreshToken);
        IEnumerable<User> GetAll();
    }
}