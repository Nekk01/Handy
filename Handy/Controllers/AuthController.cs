using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Handy.Models;
using Microsoft.AspNetCore.Mvc;

namespace Handy.Controllers
{
    [Route("")]
    public class AuthController : Controller
    {
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

        [HttpPost]
        public IActionResult LoginProceed(loginModel model)
        {
            Console.WriteLine(model.login + model.password);
            return RedirectToAction("Login");
        }
    }
}