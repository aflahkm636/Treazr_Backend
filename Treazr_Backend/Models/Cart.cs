namespace Treazr_Backend.Models
{

    public class Cart : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        // Snapshot fields
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; } = 1;

        // Snapshot of product image
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
