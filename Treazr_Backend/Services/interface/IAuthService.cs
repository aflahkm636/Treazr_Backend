using Microsoft.AspNetCore.Identity.Data;
using Treazr_Backend.DTOs.AuthDTO;

namespace Treazr_Backend.Services.interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDTO loginDto);
    }
}
