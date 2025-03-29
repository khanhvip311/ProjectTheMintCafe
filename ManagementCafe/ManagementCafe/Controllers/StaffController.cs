using ManagementCafe.Models;
using ManagementCafe.Models.FillModels;
using ManagementCafe.Services.Momo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
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
        public async Task<IActionResult> SaveBillMomo(string cartData, [FromServices] IMomoService momoService)
        {
            // Log dữ liệu nhận được
            Debug.WriteLine("Cart Data Received: " + cartData);

            // Deserialize dữ liệu JSON từ cartData
            SaveBillRequest request;
            try
            {
                request = JsonConvert.DeserializeObject<SaveBillRequest>(cartData);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Dữ liệu giỏ hàng không hợp lệ: " + ex.Message;
                return RedirectToAction("Order", "Staff");
            }

            // Kiểm tra dữ liệu đầu vào
            if (request == null)
            {
                TempData["ErrorMessage"] = "Dữ liệu hóa đơn không hợp lệ hoặc rỗng.";
                return RedirectToAction("Order", "Staff");
            }

            if (request.BillDetails == null || !request.BillDetails.Any())
            {
                TempData["ErrorMessage"] = "Danh sách chi tiết hóa đơn không được để trống.";
                return RedirectToAction("Order", "Staff");
            }

            // Kiểm tra session
            var userJson = HttpContext.Session.GetString("AccountLogOn");
            if (string.IsNullOrEmpty(userJson))
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin nhân viên. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            User user = JsonConvert.DeserializeObject<User>(userJson);
            if (user == null || user.UserId <= 0)
            {
                TempData["ErrorMessage"] = "Thông tin nhân viên không hợp lệ. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra existingUser
            var existingUser = db.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser == null)
            {
                TempData["ErrorMessage"] = $"Người dùng với ID {user.UserId} không tồn tại.";
                return RedirectToAction("Order", "Staff");
            }

            // Sử dụng transaction để lưu hóa đơn
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Tạo hóa đơn mới
                    var bill = new Bill
                    {
                        //BillId = int.Parse(DateTime.UtcNow.Ticks.ToString()),
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        Discount = request.Discount,
                        Price = request.Price,
                        TotalPrice = request.TotalPrice,
                        Status = false,
                        UserId = user.UserId,
                        PaymentMethod = request.PaymentMethod
                    };
                    db.Bills.Add(bill);
                    db.SaveChanges();

                    // Lưu chi tiết hóa đơn
                    foreach (var item in request.BillDetails)
                    {
                        var product = db.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                        if (product == null)
                        {
                            transaction.Rollback();
                            TempData["ErrorMessage"] = $"Sản phẩm với ID {item.ProductId} không tồn tại.";
                            return RedirectToAction("Order", "Staff");
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

                    // Tạo OrderId duy nhất bằng cách kết hợp bill.BillId với timestamp
                    var timestamp = DateTime.UtcNow.Ticks.ToString();
                    var uniqueOrderId = $"{bill.BillId}_{timestamp}"; // Ví dụ: "4_638787866494075861"
                    // Tạo OrderInfo
                    OrderInfo o = new OrderInfo
                    {
                        FullName = existingUser.Name,
                        OrderId = uniqueOrderId,
                        Amount = request.TotalPrice,
                        OrderInformation = $"Thanh toán hóa đơn {bill.BillId} qua Momo tại The Mint Café"
                    };
                    Debug.WriteLine("OrderInfo: " + JsonConvert.SerializeObject(o));

                    transaction.Commit();

                    // Gọi MoMo API qua MomoService
                    var momoResponse = await momoService.CreatePaymentMomo(o);
                    Debug.WriteLine("MomoResponse: " + JsonConvert.SerializeObject(momoResponse));
                    if (momoResponse == null || string.IsNullOrEmpty(momoResponse.PayUrl))
                    {
                        TempData["ErrorMessage"] = "Không thể tạo URL thanh toán MoMo.";
                        return RedirectToAction("Order", "Staff");
                    }

                    return Redirect(momoResponse.PayUrl);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi lưu hóa đơn: " + ex.Message;
                    return RedirectToAction("Order", "Staff");
                }
            }
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
