using Microsoft.AspNetCore.Mvc;

namespace Handy
{
    public class BillutinboardController : Controller
    {
        [HttpGet("Billutinboard")]
        public ActionResult Billutinboard()
        {
            

            return View();
        }
    }
}
