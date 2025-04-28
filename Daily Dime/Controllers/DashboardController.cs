using Microsoft.AspNetCore.Mvc;

namespace Daily_Dime.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
