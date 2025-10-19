namespace Treazr_Backend.DTOs.OrderDto
{
    public class ViewOrderDTO
    {
        public int Id { get; set; }

      
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentMethod { get; set; }

       
        public AddressDTO Address { get; set; }

        
        public List<OrderItemDTO> Items { get; set; }


        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
