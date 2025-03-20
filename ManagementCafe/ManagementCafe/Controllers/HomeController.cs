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
            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("AccountLogOn")); ;
            //var pr = db.Products.FirstOrDefault(t => t.ProductId == 1);
            if (u == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View(u);
            }
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
