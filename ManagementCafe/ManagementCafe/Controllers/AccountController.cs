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

        public IActionResult ForgotPass()
        {
            return View();
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["ErrorMessage"] = "";
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(IFormCollection infor)
        {
            var listuser = db.Users.ToList();
            string usernameInput = infor["Username"];
            string passwordInput = infor["Password"];

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(usernameInput) || string.IsNullOrEmpty(passwordInput))
            {
                ViewData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin đăng nhập.";
                return View();
            }

            // Tìm người dùng theo email hoặc số điện thoại
            var user = listuser.FirstOrDefault(u => u.Email == usernameInput || u.Phone == usernameInput);

            if (user != null)
            {
                if (user.Pass == passwordInput) // So sánh mật khẩu
                {
                    string ujson = JsonConvert.SerializeObject(user);
                    if (user.Role.Equals("customer"))
                    {
                        HttpContext.Session.SetString("AccountLogOn", ujson);
                        return RedirectToAction("Index", "Home");
                    }
                    else if (user.Role.Equals("staff"))
                    {
                        HttpContext.Session.SetString("AccountLogOn", ujson);
                        return RedirectToAction("Order", "Staff");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "Admin";
                        return View();
                        //return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    ViewData["ErrorMessage"] = "Mật khẩu không đúng.";
                    return View();
                }
            }
            else
            {
                ViewData["ErrorMessage"] = "Tên đăng nhập không tồn tại.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AccountLogOn");
            return RedirectToAction("Index", "Home");
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
            ViewData["ErrorMessage"] = "";
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(IFormCollection model)
        {
            // Kiểm tra tính hợp lệ của model
            if (!ModelState.IsValid)
            {
                ViewData["ErrorMessage"] = "Model không hợp lệ!";
                return View(model);
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu có khớp không
            if (model["Password"] != model["RePassword"])
            {
                ViewData["ErrorMessage"] = "Mật khẩu và mật khẩu nhập lại không khớp!";
                return View(model);
            }

            // Kiểm tra xem email hoặc số điện thoại đã tồn tại chưa
            var existingEmail = db.Users.FirstOrDefault(u => u.Email.Equals(model["Email"].ToString()));
            if (existingEmail != null)
            {
                ViewData["ErrorMessage"] = "Email đã được đăng ký!";
                return View(model);
            }

            var existingPhone = db.Users.FirstOrDefault(u => u.Phone.Equals(model["PhoneNumber"]));
            if (existingPhone != null)
            {
                ViewData["ErrorMessage"] = "Số điện thoại đã được đăng ký!";
                return View(model);
            }

            //// Hash mật khẩu trước khi lưu
            //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model["Password"]);

            // Tạo người dùng mới
            var newUser = new User
            {
                UserId = db.Users.Count() + 1,
                Name = model["FullName"],
                Phone = model["PhoneNumber"],
                Email = model["Email"],
                Address = model["Address"],
                Pass = model["Password"],
                //Gender = model.Gender,
                Role = "customer" // Vai trò mặc định là customer
            };

            // Thêm người dùng vào cơ sở dữ liệu
            try
            {
                db.Users.Add(newUser);
                db.SaveChanges();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Đăng ký thất bại: {ex.Message}";
                return View(model);
            }
        }
    }
}