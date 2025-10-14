using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Treazr_Backend.Common;
using Treazr_Backend.Models;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Controllers
{
    [Authorize(Policy ="Admin")]
    [ApiController]
    [Route("APi/[Controller]")]
    public class Usercontroller:ControllerBase
    {
        private readonly IUserService _userService;

        public Usercontroller(IUserService userservice)
        {
            _userService = userservice;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(string?search ,Roles? role)
        {
            var response = await _userService.GetAllUsersAsync(search,role);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            return StatusCode(response.StatusCode, response);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> BlockUnBlockUser(int id)
        {
            var response = await _userService.BlockUnblockUserAsync(id);
            return StatusCode(response.StatusCode, response);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            var response = await _userService.SoftDeleteUserAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
