using Treazr_Backend.Common;

namespace Treazr_Backend.Services.interfaces
{
     public interface IWishlistService
    {
        Task<ApiResponse<object>> GetWishlistAsync(int userId);
        Task<ApiResponse<string>> ToggleWishlistasync(int userId, int ProductId);

    }
}
