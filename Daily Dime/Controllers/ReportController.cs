using Microsoft.AspNetCore.Mvc;

namespace Daily_Dime.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
