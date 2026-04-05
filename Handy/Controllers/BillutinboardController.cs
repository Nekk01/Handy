using System.Data;
using System.Threading.Tasks;
using Handy.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [HttpGet("Billutinboard")]
        public async Task<ActionResult> Billutinboard()
        {
            List<BillutinboardModel> Items = new List<BillutinboardModel>();

            using (IDbConnection connection = Connection)
            {
                connection.Open();

                string sql = "SELECT ad_id, title, description, price_amount, category_id, ad_address, created_at, user_id FROM ADS;";
                using var command = new SqlCommand(sql, (SqlConnection)connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Items.Add(new BillutinboardModel
                    {
                        ID = reader.GetInt64(0),
                        ADTitle = reader.GetString(1),
                        ADDescription = reader.GetString(2),
                        ADPrice = reader.GetDecimal(3),
                        ADType = reader.GetInt64(4),
                        Address = reader.GetString(5),
                        PublicationDate = reader.GetDateTime(6),
                        UserID = reader.GetInt64(7),
                        ADPreviewImageUrl = ""
                    });
                }
            }
            return View(Items);
        }

        // Создание объявления
        [Authorize]
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

                string sql = "INSERT INTO ADS (title, description, price_amount, category_id, ad_address, user_id) VALUES (@title, @description, @price_amount, @category_id, @ad_address, 1);";
                
                using var command = new SqlCommand(sql, (SqlConnection)connection);
                
                command.Parameters.Add(new SqlParameter("@title", SqlDbType.NVarChar, 50) { Value = model.ADTitle });
                command.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 50) { Value = model.ADDescription });
                command.Parameters.Add(new SqlParameter("@price_amount", SqlDbType.NVarChar, 50) { Value = model.ADPrice });
                command.Parameters.Add(new SqlParameter("@category_id", SqlDbType.Int) { Value = model.ADType });
                command.Parameters.Add(new SqlParameter("@ad_address", SqlDbType.NVarChar, 50) { Value = model.Address });
                
                int result = command.ExecuteNonQuery();
            }
            return Redirect(Url.Action("Billutinboard", "Billutinboard"));
        }
    }
}
