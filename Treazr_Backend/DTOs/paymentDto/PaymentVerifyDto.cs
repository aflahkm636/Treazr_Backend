namespace Treazr_Backend.DTOs.paymentDto
{
    public class PaymentVerifyDto
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string Signature { get; set; }
    }
}
