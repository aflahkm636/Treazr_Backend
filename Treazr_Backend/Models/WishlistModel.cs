namespace Treazr_Backend.Models
{
    public class Wishlist : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}

