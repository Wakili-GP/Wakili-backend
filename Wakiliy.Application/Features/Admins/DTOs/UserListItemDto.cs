using Wakiliy.Domain.Enums;

namespace Wakiliy.Application.Features.Admins.DTOs
{
    public class UserListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string UserType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActionDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
