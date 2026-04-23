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
        private readonly IADService _adService;
        public BillutinboardController(IConfiguration configuration, IWebHostEnvironment environment, IADService adService)
        {
            _configuration = configuration;
            _environment = environment;
            _adService = adService;
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
            List<BillutinboardModel> Items = await _adService.GetAllRecords();

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
        public async Task<IActionResult> ADCard(int id)
        {
            BillutinboardModel ad = await _adService.GetRecord(id);

            return View(ad);
        }
        
        //создание объявления
        [HttpPost]
        public async Task<IActionResult> Submit(BillutinboardModel model, List<IFormFile> files)
        {
            bool access = await _adService.UploadRecord(model, files);

            if (access == true)
            {
                return Redirect(Url.Action("Billutinboard", "Billutinboard"));
            }
            else
            {
                return Redirect(Url.Action("Billutinboard", "Billutinboard"));
            }
        }
    }
}
