using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Treazr_Backend.Data;
using Treazr_Backend.DTOs.AuthDTO;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services.implementation
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IGenericRepository<User> _userrepo;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext appDbContext, IGenericRepository<User> userrepo, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _userrepo = userrepo;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                if (registerDto == null)
                {
                    throw new ArgumentNullException("register request cannot be null");

                }

                registerDto.Email = registerDto.Email.ToLower().Trim();
                registerDto.Name = registerDto.Name.Trim();
                registerDto.Password = registerDto.Password.Trim();


                var UserExist = await _appDbContext.Users
                    .SingleOrDefaultAsync(u=>u.Email==registerDto.Email);

                if (UserExist != null)
                {
                    return new AuthResponseDto(409, "Email already exist");
                }

                var newUser = new User
                {
                    Email = registerDto.Email,
                    Name = registerDto.Name,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Role = Roles.user
                };
                await _userrepo.AddAsync(newUser);
                return new AuthResponseDto(200, "registeration succesfull");
            }
            catch (Exception ex)
            {
                {
                    return new AuthResponseDto(500, $"error adding user {ex.Message}");
                }
            }

            
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                if (loginDTO == null)
                {
                    throw new ArgumentNullException("Login Request cannot be null");

                }
                loginDTO.Email = loginDTO.Email.Trim().ToLower();
                loginDTO.Password = loginDTO.Password.Trim();

                var user = await _appDbContext.Users
                    .SingleOrDefaultAsync(u => u.Email == loginDTO.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
                {
                    return new AuthResponseDto(401, "invalid email or  password");
                }
                else if (user.IsBlocked)
                {
                    return new AuthResponseDto(403, "This Account Has Been Blocked");
                }
                var token = GenerateJwtToken(user);
                return new AuthResponseDto(200, "Login Successful", token);


            }
            catch (Exception ex)
            {
                return new AuthResponseDto(500, $"Error while login{ex.Message}");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role,user.Role.ToString().ToLower())
            };
            var TokenDiscriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
            };
            var token = tokenHandler.CreateToken(TokenDiscriptor);
            return tokenHandler.WriteToken(token);
        }

        public Task<AuthResponseDto> LoginAsync(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }
    }
}
