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
        private readonly IWebHostEnvironment _environment;
        public BillutinboardController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }
        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("SQLConnection"));
            }
        }

        //подгрузка всех объявлений при входе на страницу
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

        //большая карточка объявления 
        [HttpGet("Billutinboard/{id?}")]
        public async Task<IActionResult> ADCard(int? id)
        {
            BillutinboardModel ad = null;
            using (IDbConnection connection = Connection)
            {
                connection.Open();

                string sql = $"SELECT ad_id, title, description, price_amount, category_id, ad_address, created_at, user_id FROM ADS WHERE ad_id = {id}";
                string sqlGetImages = $"SELECT image_path FROM AD_Image WHERE ad_id = {id}";

                using var command = new SqlCommand(sql, (SqlConnection)connection);
                using var command2 = new SqlCommand(sqlGetImages, (SqlConnection)connection);

                List<string> images = new List<string>{};
                
                using (var reader = await command2.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        images.Add(reader.GetString(0));
                    }
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        ad = new BillutinboardModel
                        {
                            ID = reader.GetInt64(0),
                            ADTitle = reader.GetString(1),
                            ADDescription = reader.GetString(2),
                            ADPrice = reader.GetDecimal(3),
                            ADType = reader.GetInt64(4),
                            Address = reader.GetString(5),
                            PublicationDate = reader.GetDateTime(6),
                            UserID = reader.GetInt64(7),
                            ADPreviewImageUrl = "",
                            Images = images
                        };
                    }
                }
            }
            return View(ad);
        }
        
        //создание объявления
        [HttpPost]
        public async Task<IActionResult> Submit(BillutinboardModel model, List<IFormFile> files)
        {
            //создание записи

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



            //загрузка файлов

            if (files == null || files.Count == 0)
            {
                return Redirect(Url.Action("Billutinboard", "Billutinboard"));
            }

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);   
            }

            using(IDbConnection connection = Connection)
            {
                connection.Open();

                string sqlGetLastAdId = "SELECT TOP 1 ad_id FROM ADS ORDER BY ad_id DESC";
                string sqlAddImages = "INSERT INTO AD_Image (image_path, ad_id) VALUES (@image_path, @ad_id);";

                using var commandGetLastAdId = new SqlCommand(sqlGetLastAdId, (SqlConnection)connection);
                using var commandSqlAddImages = new SqlCommand(sqlAddImages, (SqlConnection)connection);

                long lastAdId = 0;
                using (var reader = await commandGetLastAdId.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        lastAdId = reader.GetInt64(0);
                    }
                }

                commandSqlAddImages.Parameters.Add(new SqlParameter("@image_path", SqlDbType.NVarChar, 150));
                commandSqlAddImages.Parameters.Add(new SqlParameter("@ad_id", SqlDbType.Int));
                
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string filePath = Path.Combine(uploadsFolder, fileName);
                        string filePathSQL = "/uploads/"+fileName;

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        commandSqlAddImages.Parameters["@image_path"].Value = filePathSQL;
                        commandSqlAddImages.Parameters["@ad_id"].Value = lastAdId;

                        commandSqlAddImages.ExecuteNonQuery();
                    }
                }
            }
            return Redirect(Url.Action("Billutinboard", "Billutinboard"));
        }
    }
}
