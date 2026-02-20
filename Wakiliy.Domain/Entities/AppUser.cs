using Microsoft.AspNetCore.Identity;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Entities;
public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UploadedFile? ProfileImage { get; set; }
    public bool? AcceptTerms { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public UserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
