using Microsoft.AspNetCore.Identity;

namespace Wakiliy.Domain.Entities;
public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
