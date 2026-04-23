using System;
using System.Security.Claims;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Handy.Models;

namespace Handy;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    public AuthService(IConfiguration configuration)
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
    public async Task<ClaimsPrincipal> AuthenticateUser(string email, string password)
    {
        using(IDbConnection connection = Connection)
        {
            connection.Open();

            string sql = "SELECT user_ID, email, password FROM Users WHERE email = @Email AND password = @Password;";
            using var command = new SqlCommand(sql, (SqlConnection)connection);
                
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 50) { Value = email });
            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 20) { Value = password });
            
            object? result = await command.ExecuteScalarAsync();

            if (result == null)
            {
                return null;
            }

            var UserID = await command.ExecuteScalarAsync();
            string UserIDAsString = UserID.ToString();
            
            // Console.WriteLine(UserIDAsString);

            var Claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.NameIdentifier, UserIDAsString)
            };

            var identity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(identity);
        }
    }

    public async Task<bool> RegisterUser(string email, string phone, string password, string displayName)
    {
        using(IDbConnection connection = Connection)
        {
            connection.Open();

            string sql = "INSERT INTO Users (email, phone, password, display_name) VALUES (@Email, @Phone, @Password, @Name)";
            using var command = new SqlCommand(sql, (SqlConnection)connection);
            
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 50) { Value = email });
            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar, 20) { Value = phone });
            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 50) { Value = password });
            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 50) { Value = displayName });

            int result = command.ExecuteNonQuery();
            return true;
        }
    }
}
