using Microsoft.AspNetCore.Mvc;

namespace ManagementCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
