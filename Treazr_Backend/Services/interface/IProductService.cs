using Treazr_Backend.Common;
using Treazr_Backend.DTOs.ProductDTO;

namespace Treazr_Backend.Services.interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<IEnumerable<ProductDTO>> >GetProductsByCategoryAsync(int categoryId);
        Task<ApiResponse<ProductDTO?>> GetProductByIdAsync(int id);

    }
}
