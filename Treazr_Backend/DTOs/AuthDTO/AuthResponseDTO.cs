using System.Text.Json.Serialization;

namespace Treazr_Backend.DTOs.AuthDTO
{
    public class AuthResponseDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Token { get; set; }


        public AuthResponseDto(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
        public AuthResponseDto(int statusCode, string message, string token)
        {
            StatusCode = statusCode;
            Message = message;
            Token = token;
        }
    }
}
