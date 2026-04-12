using System;
using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Handy;

public interface IAuthService
{
    Task<ClaimsPrincipal> AuthenticateUser(string username, string password);
    Task<bool> RegisterUser(string email, string password, string displayName, string phone);
}
