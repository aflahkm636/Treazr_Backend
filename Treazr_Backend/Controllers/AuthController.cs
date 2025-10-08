
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
            var result = await _authService.LoginAsync(loginDto);

            var response = new ApiResponse<object>
            {
                StatusCode = result.StatusCode,
                Message = result.Message,
                Data = new
                {
                    accessToken = result.AccessToken,
                    refreshToken = result.RefreshToken
                }
            };

            return StatusCode(result.StatusCode, response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _authService.RefreshTokenAsync(refreshToken, ipAddress!);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var success = await _authService.RevokeTokenAsync(refreshToken, ipAddress!);

            if (!success)
                return BadRequest(new { message = "Token is invalid or already revoked" });

            return Ok(new { message = "Token revoked successfully" });
        }


    }
}
