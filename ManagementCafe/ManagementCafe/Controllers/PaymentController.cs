using ManagementCafe.Models;
using ManagementCafe.Services.Momo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ManagementCafe.Controllers
{
    public class PaymentController : Controller
    {
        private IMomoService _momoService;
        private ManagementCafeContext db = new ManagementCafeContext();
        public PaymentController(IMomoService momoService)
        {
            _momoService = momoService;

        }



        [HttpPost]
        [Route("/CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentMomo(OrderInfo model)
        {
            var response = await _momoService.CreatePaymentMomo(model);
            return Redirect(response.PayUrl);
        }

        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var query = HttpContext.Request.Query;
            Debug.WriteLine("Query String: " + HttpContext.Request.QueryString);

            var response = _momoService.PaymentExecuteAsync(query);
            Debug.WriteLine("MomoResponseCallBack: " + JsonConvert.SerializeObject(response));

            // Kiểm tra dữ liệu trả về
            if (string.IsNullOrEmpty(response.OrderId) || string.IsNullOrEmpty(response.BillId))
            {
                TempData["ErrorMessage"] = System.Web.HttpUtility.HtmlEncode("Dữ liệu trả về từ MoMo không hợp lệ.");
                return RedirectToAction("Order", "Staff");
            }

            // Tìm hóa đơn trong database bằng BillId
            var bill = db.Bills.FirstOrDefault(b => b.BillId.ToString() == response.BillId);
            if (bill == null)
            {
                TempData["ErrorMessage"] = System.Web.HttpUtility.HtmlEncode($"Không tìm thấy hóa đơn với ID {response.BillId}.");
                return RedirectToAction("Order", "Staff");
            }

            // Xử lý dựa trên resultCode (giao dịch thành công)
            if (!string.IsNullOrEmpty(response.ResultCode))
            {
                Debug.WriteLine("response.ResultCode: " + response.ResultCode);
                if (response.ResultCode == "0")
                {
                    // Giao dịch thành công
                    bill.Status = true;
                    db.SaveChanges(); // Lưu thay đổi vào database
                    TempData["SuccessMessage"] = System.Web.HttpUtility.HtmlEncode($"Thanh toán hóa đơn {response.BillId} thành công! Số tiền: {response.Amount} VNĐ.");
                }
                else if (response.ResultCode == "1006")
                {
                    // Người dùng hủy giao dịch
                    bill.Status = false;
                    db.Bills.Remove(bill);
                    db.SaveChanges(); // Lưu thay đổi vào database
                    TempData["ErrorMessage"] = System.Web.HttpUtility.HtmlEncode("Bạn đã hủy thanh toán.");
                }
                else
                {
                    // Giao dịch thất bại
                    TempData["ErrorMessage"] = System.Web.HttpUtility.HtmlEncode($"Thanh toán hóa đơn {response.BillId} thất bại. Mã lỗi: {response.ResultCode}. Thông báo: {response.Message}");
                }
            }
            // Xử lý dựa trên errorCode (giao dịch thất bại)
            else if (!string.IsNullOrEmpty(response.ErrorCode))
            {
                if (response.ErrorCode == "1006")
                {
                    // Người dùng hủy giao dịch 
                    bill.Status = false;
                    db.Bills.Remove(bill);
                    db.SaveChanges(); // Lưu thay đổi vào database
                    TempData["ErrorMessage"] = System.Web.HttpUtility.HtmlEncode("Bạn đã hủy thanh toán.");
                }
                else
                {
                    // Giao dịch thất bại
                    db.Bills.Remove(bill);
                    db.SaveChanges(); // Lưu thay đổi vào database
                    TempData["ErrorMessage"] = System.Web.HttpUtility.HtmlEncode($"Thanh toán hóa đơn {response.BillId} thất bại. Mã lỗi: {response.ErrorCode}. Thông báo: {response.Message}");
                }
            }
            else
            {
                // Không có resultCode hoặc errorCode
                db.Bills.Remove(bill);
                db.SaveChanges(); // Lưu thay đổi vào database
                TempData["ErrorMessage"] = System.Web.HttpUtility.HtmlEncode($"Thanh toán hóa đơn {response.BillId} thất bại. Thông báo: {response.Message}");
            }

            return RedirectToAction("Order", "Staff");
        }


    }
}
