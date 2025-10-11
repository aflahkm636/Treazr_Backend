using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Treazr_Backend.Common;
using Treazr_Backend.Data;
using Treazr_Backend.DTOs;
using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.Models;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ViewOrderDTO>> CreateOrderFromCartAsync(int userId, CreateOrderDTO dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get all cart items for the user, including product and its images
                var cartItems = await _context.CartItems
                    .Include(ci => ci.Product)
                        .ThenInclude(p => p.Images)
                    .Include(ci => ci.Cart)
                    .Where(ci => ci.Cart.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                    throw new Exception("Cart is empty");

                // Create order items
                var orderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price,
                    Product = ci.Product,
                    Name = ci.Product.Name
                }).ToList();

                // Remove cart items
                _context.CartItems.RemoveRange(cartItems);

                // Get or create address
                var address = await GetOrCreateAddress(dto, userId);

                // Calculate total amount
                var totalAmount = orderItems.Sum(oi => oi.Price * oi.Quantity);

                // Map order and set properties
                var order = _mapper.Map<Order>(dto);
                order.UserId = userId;
                order.TotalAmount = totalAmount;
                order.AddressId = address.Id;
                order.Items = orderItems;
                order.OrderStatus = OrderStatus.Pending;
                order.PaymentStatus = PaymentStatus.Pending;

                // Save order
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var result = _mapper.Map<ViewOrderDTO>(order);
                return new ApiResponse<ViewOrderDTO>(200, "Order placed successfully", result);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ApiResponse<ViewOrderDTO>> CreateOrderBuyNowAsync(int userId, BuyNowDTO dto, CreateOrderDTO orderDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == dto.ProductId);

                if (product == null)
                    throw new Exception("Product not found");

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = dto.Quantity,
                    Price = product.Price,
                    Name = product.Name,
                    Product = product
                };

                var orderItems = new List<OrderItem> { orderItem };

                var address = await GetOrCreateAddress(orderDto, userId);

                var totalAmount = orderItems.Sum(oi => oi.Price * oi.Quantity);

                var order = _mapper.Map<Order>(orderDto);
                order.UserId = userId;
                order.TotalAmount = totalAmount;
                order.AddressId = address.Id;
                order.Items = orderItems;
                order.OrderStatus = OrderStatus.Pending;
                order.PaymentStatus = PaymentStatus.Pending;

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var result = _mapper.Map<ViewOrderDTO>(order);
                return new ApiResponse<ViewOrderDTO>(200, "Order placed successfully", result);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }



        public async Task<ApiResponse<ViewOrderDTO>> UpdateOrderStatus(int orderId, OrderStatus newstatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            order.OrderStatus = newstatus;

            if (order.PaymentMethod == PaymentMethod.CashOnDelivery && newstatus == OrderStatus.Delivered)
            {
                order.PaymentStatus = PaymentStatus.Completed;
            }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            var orderDto = _mapper.Map<ViewOrderDTO>(order);
            return new ApiResponse<ViewOrderDTO>(200, "order status updated successfully", orderDto);

        }


        public async Task<ApiResponse<IEnumerable<ViewOrderDTO>>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<ViewOrderDTO>>(orders);
            return new ApiResponse<IEnumerable<ViewOrderDTO>>(200
, "fetched all orders succesfully", result);
        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(int userId, int OrderId)
        {
            var order = await _context.Orders
              .Include(o => o.Address)
              .Include(o => o.Items)
                  .ThenInclude(oi => oi.Product)
                      .ThenInclude(p => p.Images)
              .FirstOrDefaultAsync(o => o.Id == OrderId && o.UserId == userId);

            if (order == null)
                throw new Exception("Order not found or does not belong to the user");

            if (order.OrderStatus == OrderStatus.Delivered || order.OrderStatus == OrderStatus.Cancelled || order.OrderStatus == OrderStatus.Shipped)
                throw new Exception("Order cannot be cancelled");

            order.OrderStatus = OrderStatus.Cancelled;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(200, "order cancelled successfully", true);
        }


        public async Task<ApiResponse<PagedResult<ViewOrderDTO>>> GetAllOrdersAsync(int pagenumber, int limit)
        {
            var totalOrders = await _context.Orders.CountAsync();
            var orders = await _context.Orders
                .Include(u => u.User)
                                .Include(u => u.Address)

                .Include(u => u.Items)
    .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.Images).OrderBy(o => o.Id)

                .Skip((pagenumber - 1) * limit)
                .Take(limit)
                .ToListAsync();
            var totalpages = (int)Math.Ceiling((double)totalOrders / limit);
            var orderDTO = _mapper.Map<List<ViewOrderDTO>>(orders);
            var result = new PagedResult<ViewOrderDTO>
            {
                Items = orderDTO,

                CurrentPage = pagenumber,
                PageSize = limit,
                TotalItems = totalOrders,
                TotalPages = totalpages
            };

            return new ApiResponse<PagedResult<ViewOrderDTO>>(200, "succesfully fetched orders", result);

        }

        public async Task<ApiResponse<ViewOrderDTO>> GetOrderbyIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(u => u.User)
                .Include(u => u.Address)
                .Include(u => u.Items)
                 .ThenInclude(i => i.ImageData)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return new ApiResponse<ViewOrderDTO>(404, "Order not found");

            var result = _mapper.Map<ViewOrderDTO>(order);
            return new ApiResponse<ViewOrderDTO>(200, "Order fetched successfully", result);
        }


        public async Task<ApiResponse<IEnumerable<ViewOrderDTO>>> SearchOrdersAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return new ApiResponse<IEnumerable<ViewOrderDTO>>(400, "Username cannot be empty");

            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .Where(o => o.User != null &&
                            EF.Functions.Like(o.User.Name, $"%{username}%"))
                .ToListAsync();

            if (!orders.Any())
                return new ApiResponse<IEnumerable<ViewOrderDTO>>(404, "No orders found for the given username");

            var result = _mapper.Map<List<ViewOrderDTO>>(orders);
            return new ApiResponse<IEnumerable<ViewOrderDTO>>(200, "Orders found", result);
        }


        public async Task<ApiResponse<IEnumerable<ViewOrderDTO>>> GetOrdersByStatus(OrderStatus status)
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .Where(o => o.OrderStatus == status)
                .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return new ApiResponse<IEnumerable<ViewOrderDTO>>(404, $"No orders found with status '{status}'");
            }

            var result = _mapper.Map<List<ViewOrderDTO>>(orders);
            return new ApiResponse<IEnumerable<ViewOrderDTO>>(200, $"Orders with status '{status}' fetched successfully", result);
        }

        public async Task<ApiResponse<IEnumerable<ViewOrderDTO>>> SortOrdersByDateAsync(bool ascending)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items);

            var orders = ascending
                ? await query.OrderBy(o => o.CreatedOn).ToListAsync()
                : await query.OrderByDescending(o => o.CreatedOn).ToListAsync();

            var result = _mapper.Map<List<ViewOrderDTO>>(orders);

            string message = ascending
                ? "Orders sorted in ascending order (oldest first)"
                : "Orders sorted in descending order (recent first)";

            return new ApiResponse<IEnumerable<ViewOrderDTO>>(200, message, result);
        }

        private async Task<Address> GetOrCreateAddress(CreateOrderDTO dto, int userId)
        {
            if (dto.NewAddress != null)
            {
                var address = _mapper.Map<Address>(dto.NewAddress);
                address.UserId = userId;
                await _context.Addresses.AddAsync(address);
                await _context.SaveChangesAsync();
                return address;
            }
            else if (dto.AddressId.HasValue && dto.AddressId > 0)
            {
                return await _context.Addresses.FindAsync(dto.AddressId.Value)
                       ?? throw new Exception("Address not found");
            }
            else
            {
                throw new Exception("Address is required");
            }
        }

    }
}
