using Microsoft.AspNetCore.Mvc;

namespace EarnVidhiCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
