using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Treazr_Backend.Common;
using Treazr_Backend.Data;
using Treazr_Backend.DTOs;
using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.Models;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services
{
    public class OrderService:IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ViewOrderDTO>> CreateOrderAsync(CreateOrderDTO dto)
        {
            var user = await _context.Users
                .Include(u => u.Cart)
                    .ThenInclude(c => c.Items)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null)
                throw new Exception("User not found");

            var orderItems = new List<OrderItem>();

            if (dto.BuyNow != null)
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == dto.BuyNow.ProductId);

                if (product == null)
                    throw new Exception("Product not found");

                var orderItem = _mapper.Map<OrderItem>(dto.BuyNow);
                orderItem.Price = product.Price;
                orderItem.Product = product;

                orderItems.Add(orderItem);
            }
            else
            {
                var cart = user.Cart;
                if (cart == null || !cart.Items.Any())
                    throw new Exception("Cart is empty");

                foreach (var ci in cart.Items)
                {
                    orderItems.Add(new OrderItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        Price = ci.Product.Price,
                        Product = ci.Product
                    });
                }

                _context.CartItems.RemoveRange(cart.Items);
            }

            Address address;
            if (dto.NewAddress != null)
            {
                address = _mapper.Map<Address>(dto.NewAddress);
                address.UserId = dto.UserId;

                await _context.Addresses.AddAsync(address);
                await _context.SaveChangesAsync();
            }
            else if (dto.AddressId != null)
            {
                address = await _context.Addresses.FindAsync(dto.AddressId)
                    ?? throw new Exception("Address not found");
            }
            else
            {
                throw new Exception("Address is required");
            }

            var totalAmount = orderItems.Sum(oi => oi.Price * oi.Quantity);

            var order = _mapper.Map<Order>(dto);
            order.TotalAmount = totalAmount;
            order.AddressId = address.Id;
            order.Items = orderItems;
            order.OrderStatus = OrderStatus.Pending;
            order.PaymentStatus = PaymentStatus.Pending;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<ViewOrderDTO>(order);
            return new ApiResponse<ViewOrderDTO>(200, "order placed succesfully", result);
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

            var orderDto=_mapper.Map<ViewOrderDTO>(order);
            return new ApiResponse<ViewOrderDTO> (200,"order status updated successfully",orderDto);

        }
        

        public async Task<ApiResponse<List<ViewOrderDTO>>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .ToListAsync();

            var result= _mapper.Map<List<ViewOrderDTO>>(orders);
            return new ApiResponse<List<ViewOrderDTO>>(200
, "fetched all orders succesfully", result);
        }

        public async Task<ApiResponse<bool>>CancelOrderAsync(int userId,int OrderId)
        {
            var order = await _context.Orders
              .Include(o => o.Address)
              .Include(o => o.Items)
                  .ThenInclude(oi => oi.Product)
                      .ThenInclude(p => p.Images)
              .FirstOrDefaultAsync(o=>o.Id == OrderId && o.UserId==userId);

            if (order == null)
                throw new Exception("Order not found or does not belong to the user");

            if (order.OrderStatus == OrderStatus.Delivered || order.OrderStatus == OrderStatus.Cancelled|| order.OrderStatus==OrderStatus.Shipped)
                throw new Exception("Order cannot be cancelled");

            order.OrderStatus = OrderStatus.Cancelled;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(200, "order cancelled successfully",true);
        }
    }
}
