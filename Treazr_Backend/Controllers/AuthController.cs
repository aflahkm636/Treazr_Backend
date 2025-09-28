
using Microsoft.AspNetCore.Mvc;
using Treazr_Backend.Common;
using Treazr_Backend.DTOs.AuthDTO;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> AddUser([FromBody] RegisterDto registerDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Response = await _authService.RegisterAsync(registerDto);

            if (Response.StatusCode == 200)
            {
                return Ok(Response);
            }
            else if (Response.StatusCode == 409)
            {
                return StatusCode(StatusCodes.Status409Conflict, Response.Message);
            }
            else if (Response.StatusCode == 500)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Response.Message);
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            // Call your AuthService
            var result = await _authService.LoginAsync(loginDto);

            // Wrap the service result in ApiResponse
            var response = new ApiResponse<object>
            {
                StatusCode = result.StatusCode,
                Message = result.Message,
                Data = result.Token
            };

            // Return response with the same HTTP status code as from the service
            return StatusCode(result.StatusCode, response);
        }


    }
}
