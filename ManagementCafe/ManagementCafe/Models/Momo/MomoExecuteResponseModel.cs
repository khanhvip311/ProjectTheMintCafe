namespace ManagementCafe.Models.Momo
{
    public class MomoExecuteResponseModel
    {
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string OrderInfo { get; set; }
        public string BillId { get; set; }
        public string ResultCode { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
    }
}