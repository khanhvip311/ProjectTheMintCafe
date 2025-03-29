using ManagementCafe.Models;
using ManagementCafe.Models.Momo;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace ManagementCafe.Services.Momo
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        public MomoService(IOptions<MomoOptionModel> options)
        {
            _options = options;
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfo model)
        {
            //model.OrderId = DateTime.UtcNow.Ticks.ToString();
            model.OrderInformation = "Khách hàng: " + model.FullName + ". Nội dung: " + model.OrderInformation;
            var rawData =
            $"partnerCode={_options.Value.PartnerCode}" +
            $"&accessKey={_options.Value.AccessKey}" +
            $"&requestId={model.OrderId}" +
            $"&amount={model.Amount}" +
            $"&orderId={model.OrderId}" +
            $"&orderInfo={model.OrderInformation}" +
            $"&returnUrl={_options.Value.ReturnUrl}" +
            $"&notifyUrl={_options.Value.NotifyUrl}" +
            $"&extraData=";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = RestSharp.Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");

            // Create an object representing the request data
            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = model.OrderId,
                amount = model.Amount.ToString(),
                orderInfo = model.OrderInformation,
                requestId = model.OrderId,
                extraData = "",
                signature = signature
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            var momoResponse = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
            return momoResponse;

        }

        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            // Lấy các tham số từ query string
            var requestId = collection.FirstOrDefault(s => s.Key == "requestId").Value.ToString();
            var orderId = collection.FirstOrDefault(s => s.Key == "orderId").Value.ToString();
            var amount = collection.FirstOrDefault(s => s.Key == "amount").Value.ToString();
            var orderInfo = collection.FirstOrDefault(s => s.Key == "orderInfo").Value.ToString();
            var resultCode = collection.FirstOrDefault(s => s.Key == "resultCode").Value.ToString();
            var errorCode = collection.FirstOrDefault(s => s.Key == "errorCode").Value.ToString();
            var message = collection.FirstOrDefault(s => s.Key == "message").Value.ToString();
            var signature = collection.FirstOrDefault(s => s.Key == "signature").Value.ToString();

            // Tách billId từ OrderId (phần trước dấu "_")
            string billId = "";
            if (!string.IsNullOrEmpty(orderId))
            {
                var parts = orderId.Split('_');
                if (parts.Length > 0)
                {
                    billId = parts[0]; // Ví dụ: "1009"
                }
            }

            return new MomoExecuteResponseModel
            {
                OrderId = orderId,
                Amount = amount,
                OrderInfo = orderInfo,
                BillId = billId,
                ResultCode = resultCode,
                ErrorCode = errorCode,
                Message = message
            };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
    }

}


