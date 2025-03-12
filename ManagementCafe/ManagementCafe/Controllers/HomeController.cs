using Microsoft.AspNetCore.Mvc;

namespace ManagementCafe.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        
    }
}
