namespace Treazr_Backend.Models
{
    public class Category : BaseEntity
    {

        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
