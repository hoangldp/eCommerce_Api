using System.Collections.Generic;
using Api.Entities;

namespace Api.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
    }
}