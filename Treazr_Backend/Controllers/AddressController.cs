using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Treazr_Backend.Common;
using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Only authenticated users can access
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet]
        public async Task<IActionResult> GetUserAddresses()
        {
            var userId = GetUserId();
            var addresses = await _addressService.GetUserAddressesAsync(userId);
            return Ok(new ApiResponse<IEnumerable<AddressDTO>>(200, "Addresses retrieved successfully", addresses));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddress(int id)
        {
            var userId = GetUserId();
            try
            {
                var address = await _addressService.GetAddressByIdAsync(id, userId);
                return Ok(new ApiResponse<AddressDTO>(200, "Address retrieved successfully", address));
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<string>(404, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] AddressDTO dto)
        {
            var userId = GetUserId();
            var created = await _addressService.CreateAddressAsync(dto, userId);
            return Ok(new ApiResponse<AddressDTO>(200, "Address created successfully", created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDTO dto)
        {
            var userId = GetUserId();
            try
            {
                var updated = await _addressService.UpdateAddressAsync(id, dto, userId);
                return Ok(new ApiResponse<AddressDTO>(200, "Address updated successfully", updated));
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<string>(404, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = GetUserId();
            var deleted = await _addressService.DeleteAddressAsync(id, userId);
            if (!deleted)
                return NotFound(new ApiResponse<string>(404, "Address not found or cannot be deleted"));

            return Ok(new ApiResponse<string>(200, "Address deleted successfully"));
        }
    }
}
