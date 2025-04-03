using ManagementCafe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;

namespace ManagementCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PersonnelController : Controller
    {
        ManagementCafeContext db = new ManagementCafeContext();
        public IActionResult Index()
        {
            var listPersonnel = db.Users.ToList();
            return View(listPersonnel);
        }

        public ActionResult Details(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                string name = collection["Name"].ToString().Trim();
                string phone = collection["Phone"].ToString().Trim();
                string email = collection["Email"].ToString().Trim();
                string address = collection["Address"].ToString().Trim();
                string pass = collection["Pass"].ToString().Trim();
                string role = collection["Role"].ToString().Trim();

                if (string.IsNullOrWhiteSpace(name))
                    ModelState.AddModelError("Name", "Tên không được để trống.");
                if (string.IsNullOrWhiteSpace(phone))
                    ModelState.AddModelError("Phone", "Số điện thoại không được để trống.");
                if (string.IsNullOrWhiteSpace(email))
                    ModelState.AddModelError("Email", "Email không được để trống.");
                if (string.IsNullOrWhiteSpace(address))
                    ModelState.AddModelError("Address", "Địa chỉ không được để trống.");
                if (string.IsNullOrWhiteSpace(pass) || pass.Length < 6)
                    ModelState.AddModelError("Pass", "Mật khẩu phải có ít nhất 6 ký tự.");
                if (string.IsNullOrWhiteSpace(role))
                    ModelState.AddModelError("Role", "Vai trò không được để trống.");

                var validRoles = new List<string> { "Admin", "Staff", "Customer" };
                if (!validRoles.Contains(role))
                    ModelState.AddModelError("Role", "Vai trò không hợp lệ.");

                if (db.Users.Any(u => u.Email == email))
                    TempData["Errors"] = "Email này đã tồn tại.";
                if (db.Users.Any(u => u.Phone == phone))
                    TempData["Errors"] = TempData["Errors"] + "\nSố điện thoại này đã tồn tại.";

                if (TempData["Errors"] != null)
                {
                    return RedirectToAction(nameof(Create));
                }

                var user = new User
                {
                    UserId = db.Users.Count() + 1,
                    Name = name,
                    Phone = phone,
                    Email = email,
                    Address = address,
                    Pass = pass,
                    Role = role
                };

                db.Users.Add(user);
                db.SaveChanges();

                TempData["Message"] = "Thêm người dùng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Errors"] = "Đã xảy ra lỗi: " + ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }



        public IActionResult Edit(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.Role = new SelectList(new List<string> { "Admin", "Staff", "Customer" }, user.Role);
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(int id, IFormCollection form)
        {
            var existingUser = db.Users.Find(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            string name = form["Name"].ToString();
            string phone = form["Phone"].ToString();
            string email = form["Email"].ToString();
            string pass = form["Pass"].ToString();
            string address = form["Address"].ToString();
            string role = form["Role"].ToString();

            // Lấy danh sách vai trò hợp lệ trực tiếp
            var validRoles = new List<string> { "Admin", "Staff", "Customer" };

            if (!validRoles.Contains(role))
            {
                ModelState.AddModelError("Role", "Vai trò không hợp lệ.");
                ViewBag.Role = new SelectList(validRoles, role); // Load lại danh sách Role
                return View(existingUser);
            }

            // Kiểm tra xem có thay đổi nào không
            if (existingUser.Name == name &&
                existingUser.Phone == phone &&
                existingUser.Email == email &&
                existingUser.Pass == pass && // Cân nhắc mã hóa mật khẩu trước khi lưu
                existingUser.Address == address &&
                existingUser.Role == role)
            {
                TempData["Message"] = "Không có thay đổi nào được thực hiện.";
                return RedirectToAction("Index");
            }

            // Cập nhật thông tin nhân viên
            existingUser.Name = name;
            existingUser.Phone = phone;
            existingUser.Email = email;
            existingUser.Pass = pass;
            existingUser.Address = address;
            existingUser.Role = role;

            db.Users.Update(existingUser);
            db.SaveChanges();

            TempData["Message"] = "Cập nhật thành công.";
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult CheckDuplicate(string email, string phone)
        {
            bool emailExists = db.Users.Any(u => u.Email == email);
            bool phoneExists = db.Users.Any(u => u.Phone == phone);

            return Json(new { emailExists, phoneExists });
        }


    }
}
