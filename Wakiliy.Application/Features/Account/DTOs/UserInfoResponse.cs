using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Account.DTOs
{
    public class UserInfoResponse
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
    }
}