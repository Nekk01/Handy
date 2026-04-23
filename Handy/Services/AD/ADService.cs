using System.Data;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Handy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace Handy;

public class ADService : IADService
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ADService(IConfiguration configuration, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }
    public IDbConnection Connection
    {
        get
        {
            return new SqlConnection(_configuration.GetConnectionString("SQLConnection"));
        }
    }

    public async Task<List<BillutinboardModel>> GetAllRecords()
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

        return Items;
    }
    public async Task<BillutinboardModel> GetRecord(int id)
    {
        BillutinboardModel item = null;
        using (IDbConnection connection = Connection)
        {
            connection.Open();

            string sql = "SELECT ad_id, title, description, price_amount, category_id, ad_address, created_at, user_id FROM ADS WHERE ad_id = @id";
            string sqlGetImages = "SELECT image_path FROM AD_Image WHERE ad_id = @id";

            using var command = new SqlCommand(sql, (SqlConnection)connection);
            using var command2 = new SqlCommand(sqlGetImages, (SqlConnection)connection);

            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            command2.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

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
                    item = new BillutinboardModel
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

        return item;
    }
    public async Task<bool> UploadRecord(BillutinboardModel model, List<IFormFile> files)
    {
        //создание записи

        using(IDbConnection connection = Connection)
        {
            connection.Open();

            var user_ID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string sql = "INSERT INTO ADS (title, description, price_amount, category_id, ad_address, user_id) VALUES (@title, @description, @price_amount, @category_id, @ad_address, @user_id);";
            
            using var command = new SqlCommand(sql, (SqlConnection)connection);
            
            command.Parameters.Add(new SqlParameter("@title", SqlDbType.NVarChar, 50) { Value = model.ADTitle });
            command.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 50) { Value = model.ADDescription });
            command.Parameters.Add(new SqlParameter("@price_amount", SqlDbType.NVarChar, 50) { Value = model.ADPrice });
            command.Parameters.Add(new SqlParameter("@category_id", SqlDbType.Int) { Value = model.ADType });
            command.Parameters.Add(new SqlParameter("@ad_address", SqlDbType.NVarChar, 50) { Value = model.Address });
            command.Parameters.Add(new SqlParameter("@user_id", SqlDbType.Int) { Value = user_ID });

            int result = command.ExecuteNonQuery();
        }



        //загрузка файлов

        if (files == null || files.Count == 0)
        {
            return false;
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
        return true;
    }
}
