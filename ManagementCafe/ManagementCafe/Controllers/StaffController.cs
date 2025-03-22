using ManagementCafe.Models;
using ManagementCafe.Models.FillModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ManagementCafe.Controllers
{
    public class StaffController : Controller
    {
        private ManagementCafeContext db = new ManagementCafeContext();
        public IActionResult Order()
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
            var listcate = db.Categories.ToList();
            var listproduct = db.Products.ToList();
            var filloder = new FillOrder(listcate, listproduct);


            return View(filloder);
        }

        [HttpGet]
        public IActionResult FilterProducts(int categoryId)
        {
            var products = db.Products
                .Where(p => p.CateId == categoryId)
                .ToList();

            foreach (var p in products)
            {
                Debug.WriteLine(p.Name);
                Debug.WriteLine(p.ProductId);

            }

            // Sử dụng Newtonsoft.Json
            string jsonProducts = JsonConvert.SerializeObject(products);
            Debug.WriteLine("JSON Products: " + jsonProducts);

            return Json(products);
        }

    }
}
