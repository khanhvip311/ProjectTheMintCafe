using ManagementCafe.Models;
using ManagementCafe.Models.FillModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace ManagementCafe.Controllers
{
    public class StaffController : Controller
    {
        private ManagementCafeContext db = new ManagementCafeContext();
        private readonly ILogger<StaffController> _logger;
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

        [HttpGet]
        public IActionResult GetProductById(int productId)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product == null)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true, product });
        }





        public class SaveBillRequest
        {
            public List<BillDetailRequest> BillDetails { get; set; }
            public decimal Price { get; set; }
            public decimal TotalPrice { get; set; }
            public int Discount { get; set; }
            public int PaymentMethod { get; set; }
        }

        public class BillDetailRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public string Note { get; set; }
        }

        [HttpPost]
        public IActionResult SaveBill([FromBody] SaveBillRequest request)
        {
            try
            {
                // Log dữ liệu nhận được
                Debug.WriteLine("TestRequest: " + JsonConvert.SerializeObject(request));

                // Kiểm tra session
                var userJson = HttpContext.Session.GetString("AccountLogOn");
                if (string.IsNullOrEmpty(userJson))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin nhân viên. Vui lòng đăng nhập lại." });
                }

                User user = JsonConvert.DeserializeObject<User>(userJson);
                if (user == null || user.UserId <= 0)
                {
                    return Json(new { success = false, message = "Thông tin nhân viên không hợp lệ. Vui lòng đăng nhập lại." });
                }

                // Kiểm tra dữ liệu đầu vào
                if (request == null)
                {
                    return Json(new { success = false, message = "Dữ liệu hóa đơn không hợp lệ hoặc rỗng." });
                }

                if (request.BillDetails == null || !request.BillDetails.Any())
                {
                    return Json(new { success = false, message = "Danh sách chi tiết hóa đơn không được để trống." });
                }

                if (request.PaymentMethod != 1 && request.PaymentMethod != 2)
                {
                    return Json(new { success = false, message = "Phương thức thanh toán không hợp lệ. Chỉ chấp nhận giá trị 1 hoặc 2." });
                }

                // Kiểm tra người dùng tồn tại trong database
                var existingUser = db.Users.FirstOrDefault(u => u.UserId == user.UserId);
                if (existingUser == null)
                {
                    return Json(new { success = false, message = $"Người dùng với ID {user.UserId} không tồn tại." });
                }

                // Tạo hóa đơn mới
                var bill = new Bill
                {
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Discount = request.Discount,
                    Price = request.Price,
                    TotalPrice = request.TotalPrice,
                    Status = true,
                    UserId = user.UserId,
                    PaymentMethod = request.PaymentMethod
                };

                Debug.WriteLine("TestBillBeforeAdd: ", bill);

                db.Bills.Add(bill);
                db.SaveChanges();

                // Lưu chi tiết hóa đơn
                foreach (var item in request.BillDetails)
                {
                    var product = db.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    if (product == null)
                    {
                        return Json(new { success = false, message = $"Sản phẩm với ID {item.ProductId} không tồn tại." });
                    }

                    if (item.Quantity <= 0)
                    {
                        return Json(new { success = false, message = "Số lượng sản phẩm phải lớn hơn 0." });
                    }

                    var billDetail = new BillDetail
                    {
                        BillId = bill.BillId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Note = item.Note?.Trim()
                    };
                    db.BillDetails.Add(billDetail);
                }
                db.SaveChanges();

                return Json(new { success = true, billId = bill.BillId });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " Inner Exception: " + ex.InnerException.Message;
                }
                return Json(new { success = false, message = "Có lỗi xảy ra khi lưu hóa đơn: " + errorMessage });
            }
        }
    }
}
