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
        
        // Создание объявления
        [HttpGet("Billutinboard/Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Submit(BillutinboardModel model)
        {
            using(IDbConnection connection = Connection)
            {
                connection.Open();

                string sql = "INSERT INTO Advertisements (ADTitle, ADDescription, ADImageUrl, ADPrice, ADType, AuthorPhoneNumber, Address, PublicationDate, UserID) VALUES (@ADTitle, @ADDescription, @ADImageUrl, @ADPrice, @ADType, @AuthorPhoneNumber, @Address, GETDATE(), 1);";
                
                using var command = new SqlCommand(sql, (SqlConnection)connection);
                
                command.Parameters.Add(new SqlParameter("@ADTitle", SqlDbType.NVarChar, 50) { Value = model.ADTitle });
                command.Parameters.Add(new SqlParameter("@ADDescription", SqlDbType.NVarChar, 50) { Value = model.ADDescription });
                command.Parameters.Add(new SqlParameter("@ADImageUrl", SqlDbType.NVarChar, 50) { Value = model.ADImageUrl });
                command.Parameters.Add(new SqlParameter("@ADPrice", SqlDbType.NVarChar, 50) { Value = model.ADPrice });
                command.Parameters.Add(new SqlParameter("@ADType", SqlDbType.Int) { Value = model.ADType });
                command.Parameters.Add(new SqlParameter("@AuthorPhoneNumber", SqlDbType.NVarChar, 50) { Value = model.AuthorPhoneNumber });
                command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, 50) { Value = model.Address });
                
                int result = command.ExecuteNonQuery();
            }
            return Redirect(Url.Action("Billutinboard", "Billutinboard"));
        }
    }
}
