namespace Treazr_Backend.Models
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = "pending";
        public string PaymentMethod { get; set; }
        public string OrderStatus { get; set; } = "pending";

        public string BillingStreet { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string BillingCountry { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    }
}
