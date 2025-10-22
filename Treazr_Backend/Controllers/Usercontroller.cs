using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Treazr_Backend.Common;
using Treazr_Backend.Models;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(string? search, Roles? role)
        {
            var response = await _userService.GetAllUsersAsync(search, role);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "User")]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var loggedInUserId = GetUserId();
            var response = await _userService.GetUserByIdAsync(loggedInUserId);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> BlockUnBlockUser(int id)
        {
            var response = await _userService.BlockUnblockUserAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            var response = await _userService.SoftDeleteUserAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new UnauthorizedAccessException("User claim not found.");

            return int.Parse(claim.Value);
        }
    }
}
