using Microsoft.AspNetCore.Mvc;

namespace ManagementCafe.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Booking()
        {
            return View();
        }
    }
}
