using Microsoft.AspNetCore.Mvc;

namespace ManagementCafe.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult Order()
        {
            return View();
        }

        public IActionResult Booking()
        {
            return View();
        }
    }
}
