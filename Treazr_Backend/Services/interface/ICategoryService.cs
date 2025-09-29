using Treazr_Backend.Common;
using Treazr_Backend.DTOs.CategoryDTO;

namespace Treazr_Backend.Services.interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<IEnumerable<CategoryDTO>>> GetAllAsync();
        Task<ApiResponse<CategoryDTO?>> GetByIdAsync(int id);

        Task<ApiResponse<CategoryDTO>>AddAsync(CategoryDTO dto);

        Task<ApiResponse<CategoryDTO?>> UpdateAsync(int id, CategoryDTO dto);

        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
