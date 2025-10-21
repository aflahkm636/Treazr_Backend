using System.Threading.Tasks;
using Treazr_Backend.Common;
using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.DTOs.paymentDto;
using Treazr_Backend.Models;

namespace Treazr_Backend.Services.interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<ViewOrderDTO>> CreateOrderFromCartAsync(int userId, CreateOrderDTO dto);
        Task<ApiResponse<ViewOrderDTO>> CreateOrderBuyNowAsync(int userId, BuyNowDTO buyNowDto, CreateOrderDTO orderDto);
        Task<ApiResponse<ViewOrderDTO>> UpdateOrderStatus(int orderId, OrderStatus newStatus);
        Task<ApiResponse<IEnumerable<ViewOrderDTO>>> GetOrdersByUserIdAsync(int userId);
        Task<ApiResponse<bool>> CancelOrderAsync(int userId, int orderId);

        Task<ApiResponse<PagedResult<ViewOrderDTO>>> GetAllOrdersAsync(int pagenumber, int limit);
         Task<ApiResponse<ViewOrderDTO>> GetOrderbyIdAsync(int orderId);
        Task<ApiResponse<IEnumerable<ViewOrderDTO>>> SearchOrdersAsync(string username);
        Task<ApiResponse<IEnumerable<ViewOrderDTO>>> GetOrdersByStatus(OrderStatus status);
        Task<ApiResponse<IEnumerable<ViewOrderDTO>>> SortOrdersByDateAsync(bool ascending);
        Task<ApiResponse<object>> VerifyRazorpayPaymentAsync(PaymentVerifyDto dto);
    }
}
