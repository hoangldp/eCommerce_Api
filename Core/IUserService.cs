﻿using System.Collections.Generic;
using Data;
using Model;

namespace Core
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(string username, string password);
        AuthenticateResponse RefreshToken(string token, string refreshToken);
        IEnumerable<User> GetAll();
    }
}