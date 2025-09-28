namespace Treazr_Backend.Models
{
    public class Wishlist : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Brand { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }
}

