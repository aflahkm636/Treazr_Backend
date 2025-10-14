using Treazr_Backend.Common;
using Treazr_Backend.DTOs.ProductDTO;

namespace Treazr_Backend.Services.interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<ProductDTO>> AddProductAsync(AddProductDTO dto);
        Task<ApiResponse<object>> GetProductsByCategoryAsync(int categoryId, int? pageNumber = null, int? pageSize = null);
        Task<ApiResponse<ProductDTO?>> GetProductByIdAsync(int id);
        Task<ApiResponse<object>> GetAllProductsAsync(int? pageNumber = null, int? pageSize = null);
        Task<ApiResponse<ProductDTO>> UpdateProductASync(UpdateProductDTO dto);

        Task<ApiResponse<string>> ToggleProductStatus(int id);
        Task<ApiResponse<IEnumerable<ProductDTO>>> GetFilteredProducts(string? name);
    }
}
