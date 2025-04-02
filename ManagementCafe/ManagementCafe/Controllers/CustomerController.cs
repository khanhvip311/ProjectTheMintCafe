using ManagementCafe.Models;
using ManagementCafe.Models.FillModels;
using ManagementCafe.Services.Momo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ManagementCafe.Controllers
{
    public class CustomerController : Controller
    {
        private ManagementCafeContext db = new ManagementCafeContext();

        public IActionResult Booking()
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
            var partyTables = db.PartyTables.Where(p => p.Status == "Còn trống").ToList();
            var filloder = new FillOrder(listcate, listproduct, partyTables);


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

        public class BookingDataDetail
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public string Note { get; set; }
        }

        public class BookingData
        {
            public List<BookingDataDetail> BookingDetails { get; set; }
            public decimal Price { get; set; }
            public decimal TotalPrice { get; set; }
            public string Voucher { get; set; }
            public int PaymentMethod { get; set; }
            public int TableId { get; set; }
        }


        [HttpPost]
        public IActionResult GetVoucher(string voucherCode)
        {
            var voucher = db.Vouchers
                .FirstOrDefault(v => v.VoucherId == voucherCode);

            if (voucher == null)
            {
                return Json(null);
            }

            return Json(new
            {
                voucher = voucher.VoucherId,
                value = voucher.Value,
                description = voucher.Description,
                isPercentage = voucher.IsPercentage,
                requirement = voucher.Requirement
            });
        }

        [HttpGet]
        public IActionResult GetAvailableTables()
        {
            var availableTables = db.PartyTables
                .Where(t => t.Status == "Còn trống") // Giả sử Status = "Trống" là bàn còn trống
                .Select(t => new
                {
                    tableId = t.TableId,
                    capacity = t.Capacity,
                    status = t.Status
                })
                .ToList();

            return Json(availableTables);
        }

        [HttpPost]
        public IActionResult SaveBooking([FromBody] BookingData bookingData)
        {
            // Kiểm tra dữ liệu đã được gửi
            if (bookingData == null || bookingData.BookingDetails == null || bookingData.BookingDetails.Count == 0)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            Debug.WriteLine("bookingData: \n" + JsonConvert.SerializeObject(bookingData));

            var userJson = HttpContext.Session.GetString("AccountLogOn");
            if (string.IsNullOrEmpty(userJson))
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng. Vui lòng đăng nhập lại." });
            }

            User user = JsonConvert.DeserializeObject<User>(userJson);
            if (user == null || user.UserId <= 0)
            {
                return Json(new { success = false, message = "Thông tin khách không hợp lệ. Vui lòng đăng nhập lại." });
            }

            // Lưu dữ liệu vào cơ sở dữ liệu hoặc xử lý dữ liệu theo logic của bạn
            try
            {
                Booking bk = new Booking();
                bk.UserId = user.UserId;
                bk.BookingTime = DateTime.Now;
                bk.Status = false;
                bk.TableId = bookingData.TableId;
                bk.PaymentMethod = bookingData.PaymentMethod;
                bk.Price = bookingData.Price;
                bk.TotalPrice = bookingData.TotalPrice;
                bk.Voucher = bookingData.Voucher;



                Debug.WriteLine("Booking Data: " + JsonConvert.SerializeObject(bk));

                db.Bookings.Add(bk);
                db.SaveChanges();
                foreach (var obj in bookingData.BookingDetails)
                {
                    BookingDetail bkd = new BookingDetail();
                    bkd.BookingId = bk.BookingId;
                    bkd.ProductId = obj.ProductId;
                    bkd.Quantity = obj.Quantity;
                    bkd.Note = obj.Note;
                    db.BookingDetails.Add(bkd);
                }

                var tb = db.PartyTables.FirstOrDefault(t => t.TableId == bk.TableId);
                tb.Status = "Đang sử dụng";


                db.SaveChanges();

                // Ví dụ lưu vào cơ sở dữ liệu hoặc thực hiện xử lý
                // SaveBookingToDatabase(bookingData); 

                // Nếu lưu thành công
                return Json(new { success = true, message = "Đặt món thành công!" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Json(new { success = false, message = "Lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> SaveBillMomo(string cartData, [FromServices] IMomoService momoService)
        //{
        //    // Log dữ liệu nhận được
        //    Debug.WriteLine("Cart Data Received: " + cartData);

        //    // Deserialize dữ liệu JSON từ cartData
        //    SaveBillRequest request;
        //    try
        //    {
        //        request = JsonConvert.DeserializeObject<SaveBillRequest>(cartData);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Dữ liệu giỏ hàng không hợp lệ: " + ex.Message;
        //        return RedirectToAction("Booking", "Customer");
        //    }

        //    // Kiểm tra dữ liệu đầu vào
        //    if (request == null)
        //    {
        //        TempData["ErrorMessage"] = "Dữ liệu hóa đơn không hợp lệ hoặc rỗng.";
        //        return RedirectToAction("Booking", "Customer");
        //    }

        //    if (request.BillDetails == null || !request.BillDetails.Any())
        //    {
        //        TempData["ErrorMessage"] = "Danh sách chi tiết hóa đơn không được để trống.";
        //        return RedirectToAction("Booking", "Customer");
        //    }

        //    // Kiểm tra session
        //    var userJson = HttpContext.Session.GetString("AccountLogOn");
        //    if (string.IsNullOrEmpty(userJson))
        //    {
        //        TempData["ErrorMessage"] = "Không tìm thấy thông tin nhân viên. Vui lòng đăng nhập lại.";
        //        return RedirectToAction("Login", "Account");
        //    }

        //    User user = JsonConvert.DeserializeObject<User>(userJson);
        //    if (user == null || user.UserId <= 0)
        //    {
        //        TempData["ErrorMessage"] = "Thông tin nhân viên không hợp lệ. Vui lòng đăng nhập lại.";
        //        return RedirectToAction("Login", "Account");
        //    }

        //    // Kiểm tra existingUser
        //    var existingUser = db.Users.FirstOrDefault(u => u.UserId == user.UserId);
        //    if (existingUser == null)
        //    {
        //        TempData["ErrorMessage"] = $"Người dùng với ID {user.UserId} không tồn tại.";
        //        return RedirectToAction("Booking", "Customer");
        //    }

        //    // Sử dụng transaction để lưu hóa đơn
        //    using (var transaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            // Tạo hóa đơn mới
        //            var bill = new Bill
        //            {
        //                //BillId = int.Parse(DateTime.UtcNow.Ticks.ToString()),
        //                Date = DateOnly.FromDateTime(DateTime.Now),
        //                Discount = request.Discount,
        //                Price = request.Price,
        //                TotalPrice = request.TotalPrice,
        //                Status = false,
        //                UserId = user.UserId,
        //                PaymentMethod = request.PaymentMethod
        //            };
        //            db.Bills.Add(bill);
        //            db.SaveChanges();

        //            // Lưu chi tiết hóa đơn
        //            foreach (var item in request.BillDetails)
        //            {
        //                var product = db.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
        //                if (product == null)
        //                {
        //                    transaction.Rollback();
        //                    TempData["ErrorMessage"] = $"Sản phẩm với ID {item.ProductId} không tồn tại.";
        //                    return RedirectToAction("Booking", "Customer");
        //                }

        //                var billDetail = new BillDetail
        //                {
        //                    BillId = bill.BillId,
        //                    ProductId = item.ProductId,
        //                    Quantity = item.Quantity,
        //                    Note = item.Note?.Trim()
        //                };
        //                db.BillDetails.Add(billDetail);
        //            }
        //            db.SaveChanges();

        //            // Tạo OrderId duy nhất bằng cách kết hợp bill.BillId với timestamp
        //            var timestamp = DateTime.UtcNow.Ticks.ToString();
        //            var uniqueOrderId = $"{bill.BillId}_{timestamp}"; // Ví dụ: "4_638787866494075861"
        //            // Tạo OrderInfo
        //            OrderInfo o = new OrderInfo
        //            {
        //                FullName = existingUser.Name,
        //                OrderId = uniqueOrderId,
        //                Amount = request.TotalPrice,
        //                OrderInformation = $"Thanh toán hóa đơn {bill.BillId} qua Momo tại The Mint Café"
        //            };
        //            Debug.WriteLine("OrderInfo: " + JsonConvert.SerializeObject(o));

        //            transaction.Commit();

        //            // Gọi MoMo API qua MomoService
        //            var momoResponse = await momoService.CreatePaymentMomo(o);
        //            Debug.WriteLine("MomoResponse: " + JsonConvert.SerializeObject(momoResponse));
        //            if (momoResponse == null || string.IsNullOrEmpty(momoResponse.PayUrl))
        //            {
        //                TempData["ErrorMessage"] = "Không thể tạo URL thanh toán MoMo.";
        //                return RedirectToAction("Booking", "Customer");
        //            }

        //            return Redirect(momoResponse.PayUrl);
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            TempData["ErrorMessage"] = "Có lỗi xảy ra khi lưu hóa đơn: " + ex.Message;
        //            return RedirectToAction("Booking", "Customer");
        //        }
        //    }
        //}
    }
}
