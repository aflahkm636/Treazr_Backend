using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Treazr_Backend.DTOs.CartDTO;
using Treazr_Backend.Services.interfaces;
using System.Security.Claims;
using Treazr_Backend.Common;
using Treazr_Backend.Models;


namespace Treazr_Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController:ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("test-error")]
        [AllowAnonymous]

        public IActionResult TestError()
        {
            throw new Exception("Test exception from controller!");
        }

        [Authorize(Policy = "User")]
        [HttpPost("Add")]
        
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            int userId= GetUserId();
            var response = await _cartService.AddToCartAsync(userId, dto.ProductId, dto.Quantity);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy ="Admin")]
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var response= await _cartService.GetCartForUserAsync(userId);
            return StatusCode(response.StatusCode, response);
        }
        [Authorize(Policy ="Customer")]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            int userId = GetUserId();

            var response= await _cartService.GetCartForUserAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{cartItemId}")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateQuantityDto dto)
        {
            int userId = GetUserId();

            var response = await _cartService.UpdateCartItemAsync(userId, cartItemId, dto.quantity);

            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy ="Customer")]
        [HttpDelete("{CartItemId}")]

        public async Task<IActionResult> RemoveCartItem(int CartItemId)
        {
            int userId = GetUserId();
            var response=await _cartService.RemoveCartItemasync(userId, CartItemId);
            return StatusCode(response.StatusCode, response);


        }

        [Authorize(Policy ="Customer")]
        [HttpDelete]

        public async Task<IActionResult> ClearCart()
        {
            int userId = GetUserId();
            var response= await _cartService.ClearCartAsync(userId);
            return StatusCode(response.StatusCode, response);
        }


        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) throw new UnauthorizedAccessException("User claim not found.");
            return int.Parse(claim.Value);
        }
    }
}
