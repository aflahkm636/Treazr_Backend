using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Treazr_Backend.Models;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController:ControllerBase
    {

        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                throw new ArgumentException("user  ID not found in token");
            }
            return int.Parse(claim.Value);
        }


        [Authorize(Policy ="Customer")]
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            int userId = GetUserId();

            var response= await _wishlistService.GetWishlistAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy ="Customer")]
        [HttpPost("{productId}")]
        public async Task<IActionResult> ToggleWishlist(int productId)
        {
            int userId= GetUserId();
            var response=await _wishlistService.ToggleWishlistasync(userId, productId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
