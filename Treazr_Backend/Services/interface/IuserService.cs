using Treazr_Backend.Common;
using Treazr_Backend.Models;

namespace Treazr_Backend.Services.interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<IEnumerable<User>>> GetAllUsersAsync();
        Task<ApiResponse<User>> GetUserByIdAsync(int id);

        Task<ApiResponse<string>> BlockUnblockUserAsync(int id);
        Task<ApiResponse<string>> SoftDeleteUserAsync(int id);
    }
}
