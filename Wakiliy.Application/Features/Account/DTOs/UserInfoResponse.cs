using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Account.DTOs
{
    public class UserInfoResponse
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string UserType { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}