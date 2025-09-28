namespace Treazr_Backend.Models
{
    public class ProductImage : BaseEntity
    {
        public int ProductId { get; set; }          // FK to Product
        public Product Product { get; set; }        // Navigation property

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }   // e.g., "image/png"
        public bool IsMain { get; set; } = false;   // optional: main image shows First 
    }
}
