namespace Treazr_Backend.Models
{
    public class CartItem : BaseEntity
    {
        public int Id {  get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }
}
