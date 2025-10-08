using Microsoft.AspNetCore.Identity.Data;
using Treazr_Backend.DTOs.AuthDTO;

namespace Treazr_Backend.Services.interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDTO loginDto);

        Task<bool> RevokeTokenAsync(string token, string ipAddress);
        Task<AuthResponseDto> RefreshTokenAsync(string token, string ipAddress);
    }
}
