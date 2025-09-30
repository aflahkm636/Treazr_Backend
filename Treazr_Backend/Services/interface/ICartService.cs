using Treazr_Backend.Common;

namespace Treazr_Backend.Services.interfaces
{
    public interface ICartService
    {
        Task<ApiResponse<object>> GetCartForUserAsync (int  userId);

        Task<ApiResponse<string>> AddToCartAsync(int userId, int ProductId, int Quantity);

        Task<ApiResponse<string>> UpdateCartItemAsync(int userId,int CartItemId,int Quantity);

        Task<ApiResponse<string>> RemoveCartItemasync(int userId,int CartItemId);

        Task<ApiResponse<string>> ClearCartAsync(int userId);    

    }
}
