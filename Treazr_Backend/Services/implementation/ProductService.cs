using Treazr_Backend.Common;
using Treazr_Backend.DTOs.ProductDTO;
using Treazr_Backend.Repository.interfaces;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services.implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        // ✅ Get Products by Category
        public async Task<ApiResponse<IEnumerable<ProductDTO>>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var products = await _productRepo.GetProductsByCategoryAsync(categoryId);

                var result = products.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Brand = p.Brand,
                    InStock = p.InStock,
                    CategoryName = p.Category.Name,
                    ImageUrl = p.Images.FirstOrDefault(i => i.IsMain)?.ImageData != null
                        ? $"data:{p.Images.FirstOrDefault(i => i.IsMain)?.ImageMimeType};base64," +
                          Convert.ToBase64String(p.Images.FirstOrDefault(i => i.IsMain)!.ImageData)
                        : null
                }).ToList();

                return new ApiResponse<IEnumerable<ProductDTO>>(200, "Product Retrived Successfully", result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ProductDTO>>(500, $"Error fetching products: {ex.Message}");
            }
        }

        // ✅ Get Product by Id
        public async Task<ApiResponse<ProductDTO?>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productRepo.GetByIdAsync(id);

                if (product == null)
                    return new ApiResponse<ProductDTO?>(404, $"Product not found with id: {id}");

                var result = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Brand = product.Brand,
                    InStock = product.InStock,
                    CategoryName = product.Category.Name,
                    ImageUrl = product.Images.FirstOrDefault(i => i.IsMain)?.ImageData != null
                        ? $"data:{product.Images.FirstOrDefault(i => i.IsMain)?.ImageMimeType};base64," +
                          Convert.ToBase64String(product.Images.FirstOrDefault(i => i.IsMain)!.ImageData)
                        : null
                };

                return new ApiResponse<ProductDTO?>(200, "Product Retrived Successfully", result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductDTO?>(500, $"Error fetching product: {ex.Message}");
            }
        }
    }
}
