using AutoMapper;
using Microsoft.Extensions.Options;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;
using Treazr_Backend.Common;
using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.DTOs.paymentDto;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services
{
    using RazorpayOrder = Razorpay.Api.Order;
    using TreazrOrder = Treazr_Backend.Models.Order;

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IAddressRepository _addressRepo;
        private readonly IMapper _mapper;
        private readonly RazorpaySettings _razorpaySettings;

        public OrderService(
            IOrderRepository orderRepo,
            IAddressRepository addressRepo,
            IMapper mapper,
            IOptions<RazorpaySettings> razorpaySettings)
        {
            _orderRepo = orderRepo;
            _addressRepo = addressRepo;
            _mapper = mapper;
            _razorpaySettings = razorpaySettings.Value;
        }

        public async Task<ApiResponse<ViewOrderDTO>> CreateOrderFromCartAsync(int userId, CreateOrderDTO dto)
        {
            var cartItems = await _orderRepo.GetCartItemsByUserAsync(userId);
            if (!cartItems.Any())
                return new ApiResponse<ViewOrderDTO>(400, "Cart is empty");

            decimal totalAmount = 0;
            foreach (var item in cartItems)
            {
                if (item.Product == null)
                    return new ApiResponse<ViewOrderDTO>(404, $"Product {item.ProductId} not found");
                if (item.Product.CurrentStock < item.Quantity)
                    return new ApiResponse<ViewOrderDTO>(400, $"Not enough stock for {item.Product.Name}");

                totalAmount += item.Product.Price * item.Quantity;
                item.Product.CurrentStock -= item.Quantity;
            }

            var address = await _addressRepo.GetOrCreateAddressAsync(dto, userId);

            var orderItems = cartItems.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                Price = ci.Product.Price,
                Name = ci.Product.Name,
                Product = ci.Product
            }).ToList();

            var order = _mapper.Map<TreazrOrder>(dto);
            order.UserId = userId;
            order.TotalAmount = totalAmount;
            order.AddressId = address.Id;
            order.Items = orderItems;
            order.CreatedOn = DateTime.UtcNow;

            if (dto.PaymentMethod == PaymentMethod.CashOnDelivery)
            {
                order.PaymentStatus = PaymentStatus.Pending;
                order.OrderStatus = OrderStatus.Processing;
            }
            else if (dto.PaymentMethod == PaymentMethod.Razorpay)
            {
                var razorpayOrder = CreateRazorpayOrder(totalAmount);
                order.RazorpayOrderId = razorpayOrder["id"].ToString();
                order.PaymentStatus = PaymentStatus.Pending;
                order.OrderStatus = OrderStatus.Pending;
            }

            await _orderRepo.DeleteCartItemsAsync(cartItems);
            await _orderRepo.CreateOrderAsync(order);

            var result = _mapper.Map<ViewOrderDTO>(order);
            return new ApiResponse<ViewOrderDTO>(200, "Order created successfully", result);
        }

        public async Task<ApiResponse<ViewOrderDTO>> CreateOrderBuyNowAsync(int userId, BuyNowDTO dto, CreateOrderDTO orderDto)
        {
            var product = await _orderRepo.GetProductByIdAsync(dto.ProductId);
            if (product == null)
                return new ApiResponse<ViewOrderDTO>(404, "Product not found");

            if (product.CurrentStock < dto.Quantity)
                return new ApiResponse<ViewOrderDTO>(400, $"Not enough stock for {product.Name}");

            decimal totalAmount = product.Price * dto.Quantity;
            product.CurrentStock -= dto.Quantity;

            var address = await _addressRepo.GetOrCreateAddressAsync(orderDto, userId);

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = dto.Quantity,
                Price = product.Price,
                Name = product.Name,
                Product = product
            };

            var order = _mapper.Map<TreazrOrder>(orderDto);
            order.UserId = userId;
            order.TotalAmount = totalAmount;
            order.AddressId = address.Id;
            order.Items = new List<OrderItem> { orderItem };
            order.CreatedOn = DateTime.UtcNow;

            if (orderDto.PaymentMethod == PaymentMethod.CashOnDelivery)
            {
                order.PaymentStatus = PaymentStatus.Pending;
                order.OrderStatus = OrderStatus.Processing;
            }
            else if (orderDto.PaymentMethod == PaymentMethod.Razorpay)
            {
                var razorpayOrder = CreateRazorpayOrder(totalAmount);
                order.RazorpayOrderId = razorpayOrder["id"].ToString();
                order.PaymentStatus = PaymentStatus.Pending;
                order.OrderStatus = OrderStatus.Pending;
            }

            await _orderRepo.CreateOrderAsync(order);

            var result = _mapper.Map<ViewOrderDTO>(order);
            return new ApiResponse<ViewOrderDTO>(200, "Order placed successfully", result);
        }

        private RazorpayOrder CreateRazorpayOrder(decimal amount)
        {
            var client = new RazorpayClient(_razorpaySettings.Key, _razorpaySettings.Secret);
            var options = new Dictionary<string, object>
            {
                { "amount", (int)(amount * 100) },
                { "currency", "INR" },
                { "receipt", Guid.NewGuid().ToString() },
                { "payment_capture", 1 }
            };
            return client.Order.Create(options);
        }

        public async Task<ApiResponse<object>> VerifyRazorpayPaymentAsync(PaymentVerifyDto dto)
        {
            if (!RazorpayUtils.VerifyPaymentSignature(dto.OrderId, dto.PaymentId, dto.Signature, _razorpaySettings.Secret))
                return new ApiResponse<object>(400, "Invalid payment signature");

            var order = await _orderRepo.GetByRazorpayOrderIdAsync(dto.OrderId);
            if (order == null)
                return new ApiResponse<object>(404, "Order not found");

            order.PaymentStatus = PaymentStatus.Completed;
            order.PaymentId = dto.PaymentId;
            order.ModifiedOn = DateTime.UtcNow;

            await _orderRepo.UpdateOrderAsync(order);

            return new ApiResponse<object>(200, "Payment verified successfully");
        }

            public async Task<ApiResponse<ViewOrderDTO>> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null)
                return new ApiResponse<ViewOrderDTO>(404, "Order not found");

            order.OrderStatus = newStatus;
            if (order.PaymentMethod == PaymentMethod.CashOnDelivery && newStatus == OrderStatus.Delivered)
                order.PaymentStatus = PaymentStatus.Completed;

            await _orderRepo.UpdateOrderAsync(order);
            return new ApiResponse<ViewOrderDTO>(200, "Order status updated successfully", _mapper.Map<ViewOrderDTO>(order));
        }

        public async Task<ApiResponse<IEnumerable<ViewOrderDTO>>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);
            var result = _mapper.Map<IEnumerable<ViewOrderDTO>>(orders);
            return new ApiResponse<IEnumerable<ViewOrderDTO>>(200, "Fetched all orders successfully", result);
        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(int userId, int orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null || order.UserId != userId)
                return new ApiResponse<bool>(404, "Order not found or does not belong to the user", false);

            if (order.OrderStatus == OrderStatus.Delivered || order.OrderStatus == OrderStatus.Cancelled || order.OrderStatus == OrderStatus.Shipped)
                return new ApiResponse<bool>(400, "Order cannot be cancelled", false);

            order.OrderStatus = OrderStatus.Cancelled;
            await _orderRepo.UpdateOrderAsync(order);
            return new ApiResponse<bool>(200, "Order cancelled successfully", true);
        }

        public async Task<ApiResponse<PagedResult<ViewOrderDTO>>> GetAllOrdersAsync(int pageNumber, int limit)
        {
            var totalOrders = await _orderRepo.GetOrdersCountAsync();
            var orders = await _orderRepo.GetAllOrdersAsync(pageNumber, limit);

            var totalPages = (int)Math.Ceiling((double)totalOrders / limit);
            var result = new PagedResult<ViewOrderDTO>
            {
                Items = _mapper.Map<List<ViewOrderDTO>>(orders),
                CurrentPage = pageNumber,
                PageSize = limit,
                TotalItems = totalOrders,
                TotalPages = totalPages
            };
            return new ApiResponse<PagedResult<ViewOrderDTO>>(200, "Successfully fetched orders", result);
        }

        public async Task<ApiResponse<ViewOrderDTO>> GetOrderbyIdAsync(int orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null)
                return new ApiResponse<ViewOrderDTO>(404, "Order not found");

            return new ApiResponse<ViewOrderDTO>(200, "Order fetched successfully", _mapper.Map<ViewOrderDTO>(order));
        }

        public async Task<ApiResponse<IEnumerable<ViewOrderDTO>>> SearchOrdersAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return new ApiResponse<IEnumerable<ViewOrderDTO>>(400, "Username cannot be empty");

            var orders = await _orderRepo.SearchOrdersAsync(username);
            if (!orders.Any())
                return new ApiResponse<IEnumerable<ViewOrderDTO>>(404, "No orders found for the given username");

            return new ApiResponse<IEnumerable<ViewOrderDTO>>(200, "Orders found", _mapper.Map<List<ViewOrderDTO>>(orders));
        }

        public async Task<ApiResponse<IEnumerable<ViewOrderDTO>>> GetOrdersByStatus(OrderStatus status)
        {
            var orders = await _orderRepo.GetOrdersByStatusAsync(status);
            if (!orders.Any())
                return new ApiResponse<IEnumerable<ViewOrderDTO>>(404, $"No orders found with status '{status}'");

            return new ApiResponse<IEnumerable<ViewOrderDTO>>(200, $"Orders with status '{status}' fetched successfully", _mapper.Map<List<ViewOrderDTO>>(orders));
        }

        public async Task<ApiResponse<IEnumerable<ViewOrderDTO>>> SortOrdersByDateAsync(bool ascending)
        {
            var orders = await _orderRepo.SortOrdersByDateAsync(ascending);
            string message = ascending
                ? "Orders sorted in ascending order (oldest first)"
                : "Orders sorted in descending order (recent first)";

            return new ApiResponse<IEnumerable<ViewOrderDTO>>(200, message, _mapper.Map<List<ViewOrderDTO>>(orders));
        }


    }
}
      public static class RazorpayUtils
{
    public static bool VerifyPaymentSignature(string orderId, string paymentId, string signature, string secret)
    {
        string payload = $"{orderId}|{paymentId}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        string generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
        return generatedSignature == signature.ToLower();
    }


    }

