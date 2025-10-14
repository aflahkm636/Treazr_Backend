using System.ComponentModel.DataAnnotations;

namespace Treazr_Backend.DTOs.ProductDTO
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Brand { get; set; }

        public bool InStock { get; set; } = true;


        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string CategoryName { get; set; }

        [Required]
        public int CategoryId { get; set; }

    
        public int CurrentStock {  get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one image is required.")]
        public List<string> ImageBase64 { get; set; } = new List<string>();
    }
}
