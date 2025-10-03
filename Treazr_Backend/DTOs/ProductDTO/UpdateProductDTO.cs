using System.ComponentModel.DataAnnotations;

namespace Treazr_Backend.DTOs.ProductDTO
{
    public class UpdateProductDTO
    {
        [Required]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }


        public string? Brand { get; set; }
        public int? CategoryId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int? CurrentStock { get; set; }

        // Optional: activate/deactivate product
        public bool? IsActive { get; set; }


        // Optional: new images to add
        public List<IFormFile>? NewImages { get; set; }
    }
}
