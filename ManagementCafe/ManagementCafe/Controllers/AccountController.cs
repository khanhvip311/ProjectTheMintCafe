using ManagementCafe.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManagementCafe.Controllers
{
    public class AccountController : Controller
    {   
        ManagementCafeContext db = new ManagementCafeContext();

        public IActionResult PersonalInfo()
        {
            return View();
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(IFormCollection infor)
        {   
            var listuser = db.Users.ToList();

            if (!ModelState.IsValid)
            {
                return View(infor);
            }

            // Kiểm tra đăng nhập
            User u = new User();   
            foreach (var user in listuser) {
                if (user.Email == infor["Username"] || user.Phone == infor["Username"]) {
                   u = user;
                }
            }
            if (u != null){
                if (u.Pass == infor["Password"])
                {
                    string ujson = JsonConvert.SerializeObject(u);
                    HttpContext.Session.SetString("AccountLogOn", ujson);
                    // Session["AccountLogOn"] = u;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(); 
                }
            }
            else
            {
                return View();
            }
           
        }

        // Phương thức giả lập kiểm tra thông tin người dùng
        private bool CheckUserCredentials(string username, string password)
        {
            // Thay bằng logic thực tế (ví dụ: kiểm tra trong database)
            return username == "admin" && password == "1";
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra mật khẩu khớp nhau
            if (model.Password != model.RePassword)
            {
                ViewData["ErrorMessage"] = "Mật khẩu và mật khẩu nhập lại không khớp!";
                return View(model);
            }

            // Kiểm tra xem email đã tồn tại chưa (giả lập)
            if (IsEmailRegistered(model.Email))
            {
                ViewData["ErrorMessage"] = "Email đã được đăng ký!";
                return View(model);
            }

            // Lưu thông tin người dùng (giả lập)
            SaveUser(model);

            // Đăng ký thành công, chuyển hướng đến trang đăng nhập
            return RedirectToAction("Login", "Account");
        }

        // Phương thức giả lập kiểm tra email
        private bool IsEmailRegistered(string email)
        {
            // Trong thực tế, kiểm tra trong database
            // Giả lập: trả về false (chưa tồn tại)
            return false;
        }

        // Phương thức giả lập lưu người dùng
        private void SaveUser(RegisterViewModel model)
        {
            // Trong thực tế, lưu vào database
            // Giả lập: chỉ in ra console
            Console.WriteLine($"Registered: {model.FullName}, {model.Email}, {model.Gender}");
        }
    }
}