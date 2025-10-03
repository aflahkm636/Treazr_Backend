using Treazr_Backend.Common;
using Treazr_Backend.DTOs.ProductDTO;

namespace Treazr_Backend.Services.interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<ProductDTO>> AddProductAsync(AddProductDTO dto);
        Task<ApiResponse<IEnumerable<ProductDTO>> >GetProductsByCategoryAsync(int categoryId);
        Task<ApiResponse<ProductDTO?>> GetProductByIdAsync(int id);

        Task<ApiResponse<ProductDTO>> UpdateProductASync(UpdateProductDTO dto);

        Task<ApiResponse<string>> ToggleProductStatus(int id);
    }
}
