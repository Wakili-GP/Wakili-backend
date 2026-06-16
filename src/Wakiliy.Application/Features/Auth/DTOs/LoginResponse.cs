using Wakiliy.Application.Features.Auth.DTOs;

namespace Wakiliy.Application.Features.Auth.DTOs
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public long ExpiresIn { get; set; }
        public UserDto User { get; set; } = null!;
    }
}
