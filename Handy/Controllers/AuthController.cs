using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Handy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Handy.Controllers
{
    [Route("")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("SQLConnection"));
            }
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
            using(IDbConnection connection = Connection)
            {
                connection.Open();

                string sql = "SELECT Name, Password FROM Users WHERE Name = @Name AND Password = @Password;";
                using var command = new SqlCommand(sql, (SqlConnection)connection);
                
                command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 50) { Value = model.login });
                command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 20) { Value = model.password });
                
                object? result = await command.ExecuteScalarAsync();

                if (result == null)
                {
                    Console.WriteLine("result null");
                    return BadRequest();
                }

                return Ok();
            }
        }

        [HttpPost("register")]
        public IActionResult RegisterProceed(RegistrationModel model)
        {   
            using(IDbConnection connection = Connection)
            {
                connection.Open();

                string sql = "INSERT INTO Users (Email, Phone, Password, Name) VALUES (@Email, @Phone, @Password, @Name)";
                using var command = new SqlCommand(sql, (SqlConnection)connection);
                
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 50) { Value = model.email });
                command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar, 20) { Value = model.phone });
                command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 50) { Value = model.password });
                command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 50) { Value = model.login });

                int result = command.ExecuteNonQuery();
                return Ok();
            }
        }
    }
}