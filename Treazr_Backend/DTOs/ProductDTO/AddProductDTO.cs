using System.ComponentModel.DataAnnotations;

namespace Treazr_Backend.DTOs.ProductDTO
{
    public class AddProductDTO
    {
        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

      

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int CurrentStock { get; set; }

        [Required(ErrorMessage = "At least one image is required")]
        [MinLength(1, ErrorMessage = "At least one image is required")]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
