using Treazr_Backend.Common;
using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.Models;

namespace Treazr_Backend.Services.interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<ViewOrderDTO>> CreateOrderAsync(CreateOrderDTO dto);
        Task<ApiResponse<ViewOrderDTO>> UpdateOrderStatus(int orderId, OrderStatus newStatus);
        Task<ApiResponse<List<ViewOrderDTO>>> GetOrdersByUserIdAsync(int userId);
        Task<ApiResponse<bool>> CancelOrderAsync(int userId, int orderId);
    }
}
