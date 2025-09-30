namespace Treazr_Backend.DTOs.CartDTO
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; } = 1;
    }
}
