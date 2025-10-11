using Treazr_Backend.Models;

namespace Treazr_Backend.DTOs.OrderDto
{
    public class CreateOrderDTO
    {
        // Either existing address
        public int? AddressId { get; set; }

        // Or provide new address
        public AddressDTO? NewAddress { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
    }

    public class BuyNowDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }


}
