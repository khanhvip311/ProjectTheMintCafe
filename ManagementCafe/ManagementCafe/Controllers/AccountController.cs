using Microsoft.AspNetCore.Mvc;

namespace ManagementCafe.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult PersonalInfo()
        {
            return View();
        }
    }
}
