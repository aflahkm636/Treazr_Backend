namespace Treazr_Backend.Models
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        // Snapshot fields
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }

        // Snapshot of product image
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }
}
