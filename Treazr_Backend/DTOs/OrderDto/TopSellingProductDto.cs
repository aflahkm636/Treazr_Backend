namespace Treazr_Backend.DTOs.OrderDto
{
    public class TopSellingProductDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public int OrdersCount { get; set; }
        //public byte[]? ImageData { get; set; }
    }
}
