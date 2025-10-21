using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.Models;

namespace Treazr_Backend.Repository.interfaces
{
    public interface IOrderRepository
    {
        Task<List<CartItem>> GetCartItemsByUserAsync(int userId);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<List<Order>> GetAllOrdersAsync(int pageNumber, int limit);
        Task<int> GetOrdersCountAsync();
        Task UpdateOrderAsync(Order order);
        Task<bool> DeleteCartItemsAsync(IEnumerable<CartItem> cartItems);
        Task<List<Order>> SearchOrdersAsync(string username);
        Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<List<Order>> SortOrdersByDateAsync(bool ascending);

        Task<Order?> GetByRazorpayOrderIdAsync(string razorpayOrderId);

    }
}
