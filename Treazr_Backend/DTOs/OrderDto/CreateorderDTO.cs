using Treazr_Backend.Models;

namespace Treazr_Backend.DTOs.OrderDto
{
    public class CreateOrderDTO
    {
        public int UserId { get; set; }

        // Either existing address
        public int? AddressId { get; set; }

        // Or provide new address
        public AddressDTO? NewAddress { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        // Optional: Buy a single product directly
        public BuyNowDTO? BuyNow { get; set; }

        // Cart items will be automatically taken from user's cart if BuyNow is null
    }

    public class BuyNowDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }


}
