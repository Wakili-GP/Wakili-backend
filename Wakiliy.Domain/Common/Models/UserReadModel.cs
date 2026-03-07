using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Common.Models
{
    public class UserReadModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string UserType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActionDate { get; set; }
        public UserStatus Status { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
