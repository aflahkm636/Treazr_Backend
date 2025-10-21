using Microsoft.EntityFrameworkCore;
using Treazr_Backend.Data;
using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;

namespace Treazr_Backend.Repository.implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetCartItemsByUserAsync(int userId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.Images)
                .Include(ci => ci.Cart)
                .Where(ci => ci.Cart.UserId == userId)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync(int pageNumber, int limit)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .OrderBy(o => o.Id)
                .Skip((pageNumber - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> GetOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteCartItemsAsync(IEnumerable<CartItem> cartItems)
        {
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Order>> SearchOrdersAsync(string username)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .Where(o => o.User != null && EF.Functions.Like(o.User.Name, $"%{username}%"))
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .Where(o => o.OrderStatus == status)
                .ToListAsync();
        }

        public async Task<List<Order>> SortOrdersByDateAsync(bool ascending)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items);

            return ascending
                ? await query.OrderBy(o => o.CreatedOn).ToListAsync()
                : await query.OrderByDescending(o => o.CreatedOn).ToListAsync();
        }

        public async Task<Order?> GetByRazorpayOrderIdAsync(string razorpayOrderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.RazorpayOrderId == razorpayOrderId);
        }
    }
}
