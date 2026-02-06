using Microsoft.AspNetCore.Identity;

namespace Wakiliy.Domain.Entities;
public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool? AcceptTerms { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
