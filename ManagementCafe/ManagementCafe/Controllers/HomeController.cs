using ManagementCafe.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManagementCafe.Controllers
{

    public class HomeController : Controller
    {
        ManagementCafeContext db = new ManagementCafeContext();

        public IActionResult Index()
        {
            string userJson = HttpContext.Session.GetString("AccountLogOn");
            if (string.IsNullOrEmpty(userJson)) // Kiểm tra session có tồn tại không
            {
                return RedirectToAction("Login", "Account");
            }

            User u = JsonConvert.DeserializeObject<User>(userJson);
            if (u == null) // Kiểm tra thêm trường hợp deserialize ra null (dự phòng)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(u);
        }

        public IActionResult TermsOfService()
        {
            return View();
        }

        public IActionResult PrivacyPolicy()
        {
            return View();
        }
    }
}
