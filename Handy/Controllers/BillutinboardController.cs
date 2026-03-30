using System.Data;
using System.Threading.Tasks;
using Handy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Handy
{
    public class BillutinboardController : Controller
    {
        private readonly IConfiguration _configuration;
        public BillutinboardController(IConfiguration configuration)
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
        [HttpGet("Billutinboard")]
        public async Task<ActionResult> Billutinboard()
        {
            List<BillutinboardModel> Items = new List<BillutinboardModel>();

            using (IDbConnection connection = Connection)
            {
                connection.Open();

                string sql = "SELECT ID, ADTitle, ADDescription, ADImageUrl, ADPrice, ADType, AuthorPhoneNumber, Address, PublicationDate, UserID FROM Advertisements;";
                using var command = new SqlCommand(sql, (SqlConnection)connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Items.Add(new BillutinboardModel
                    {
                        ID = reader.GetInt32(0),
                        ADTitle = reader.GetString(1),
                        ADDescription = reader.GetString(2),
                        ADImageUrl = reader.GetString(3),
                        ADPrice = reader.GetString(4),
                        ADType = reader.GetInt32(5),
                        AuthorPhoneNumber = reader.GetString(6),
                        Address = reader.GetString(7),
                        PublicationDate = reader.GetDateTime(8),
                        UserID = reader.GetInt32(9)
                    });
                }
            }
            return View(Items);
        }
    }
}
