namespace Treazr_Backend.Models
{

    public class Cart : BaseEntity
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public User User { get; set; }

        // Navigation property
        public List<CartItem> Items { get; set; } = new List<CartItem>();
     


    }
}
