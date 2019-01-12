using Microsoft.AspNetCore.Mvc;

namespace MyCourse.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}