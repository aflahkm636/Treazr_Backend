namespace Treazr_Backend.DTOs.OrderDto
{
    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<string> Images { get; set; }
    }
}
