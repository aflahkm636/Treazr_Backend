using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Treazr_Backend.Common;
using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.DTOs.paymentDto;
using Treazr_Backend.Models;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) throw new UnauthorizedAccessException("User claim not found.");
            return int.Parse(claim.Value);
        }

    
        [HttpPost("checkout/cart")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> CreateOrderFromCart([FromBody] CreateOrderDTO dto)
        {
            var userId = GetUserId();
            var response = await _orderService.CreateOrderFromCartAsync(userId, dto);
            return StatusCode(response.StatusCode, response);
        }

       
        [HttpPost("checkout/buynow")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> CreateOrderBuyNow([FromBody] BuyNowRequestDTO dto)
        {
            var userId = GetUserId();
            var response = await _orderService.CreateOrderBuyNowAsync(userId, dto.BuyNow, dto.Order);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("verify-payment")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> VerifyRazorpayPayment([FromBody] PaymentVerifyDto dto)
        {
            var response = await _orderService.VerifyRazorpayPaymentAsync(dto);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("admin/update-status/{orderId}")]
        [Authorize(Policy ="Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, newStatus);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Authorize(Policy ="Customer")]

        public async Task<IActionResult>GetuserOrders()
        {
            var userId = GetUserId();
            var response = await _orderService.GetOrdersByUserIdAsync(userId);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("user/{userId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var response = await _orderService.GetOrdersByUserIdAsync(userId);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("cancel/{orderId}")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult>CancelOrder(int orderId)
        {
            var userId=GetUserId();
            var response=await _orderService.CancelOrderAsync(userId,orderId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("all")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAllOrders([FromQuery] int pageNumber = 1, [FromQuery] int limit = 10)
        {
            var response = await _orderService.GetAllOrdersAsync(pageNumber, limit);
            return StatusCode(response.StatusCode, response);
        }


        [HttpGet("{orderId}")]
        [Authorize(Policy = "user")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var response = await _orderService.GetOrderbyIdAsync(orderId);
            return StatusCode(response.StatusCode, response);
        }


        [HttpGet("search")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> SearchOrders([FromQuery] string username)
        {
            var response = await _orderService.SearchOrdersAsync(username);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("status/{status}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetOrdersByStatus(OrderStatus status)
        {
            var response = await _orderService.GetOrdersByStatus(status);
            return StatusCode(response.StatusCode, response);
        }



        [HttpGet("sort")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> SortOrdersByDate([FromQuery] bool ascending = true)
        {
            var response = await _orderService.SortOrdersByDateAsync(ascending);
            return StatusCode(response.StatusCode, response);
        }

    }

    
    public class BuyNowRequestDTO
    {
        public BuyNowDTO BuyNow { get; set; } = default!;
        public CreateOrderDTO Order { get; set; } = default!;
    }
}
