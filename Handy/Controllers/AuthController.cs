using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;
using Handy.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Handy.Controllers
{
    [Route("")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService AuthService)
        {
            _authService = AuthService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginProceed(loginModel model)
        {
            var principal = await _authService.AuthenticateUser(model.email, model.password);

            if (principal == null)
            {
                await HttpContext.SignInAsync("Cookie", principal);
                return null;
            }

            await HttpContext.SignInAsync("Cookie", principal);
            return Redirect(Url.Action("Billutinboard", "Billutinboard"));
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterProceed(RegistrationModel model)
        {   
            bool RegistrationResult = await _authService.RegisterUser(model.email, model.phone, model.password, model.displayName);

            if (RegistrationResult == true)
            {
                return Redirect(Url.Action("login", "Auth"));
            }

            return Redirect(Url.Action("Register", "Auth"));
        }
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.SignOutAsync("Cookie");
            return Redirect(Url.Action("Billutinboard", "Billutinboard"));
        }
    }
}