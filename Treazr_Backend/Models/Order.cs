using Microsoft.EntityFrameworkCore;

namespace Treazr_Backend.Models
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        // Reference saved billing address
        public int BillingAddressId { get; set; }
        public Address BillingAddress { get; set; }

        // Order items
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();


    }
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public enum PaymentMethod
    {
        CashOnDelivery,
        Razorpay
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

}
