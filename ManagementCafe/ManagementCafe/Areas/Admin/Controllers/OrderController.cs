using ManagementCafe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        ManagementCafeContext db = new ManagementCafeContext();
        public IActionResult Index()
        {
            var listOrder = db.Bills.ToList();
            return View(listOrder);
        }

        public ActionResult Details(int id)
        {
            var bill = db.Bills.Find(id);
            if (bill == null)
            {
                return NotFound();
            }
            return View(bill);
        }


    }
}
