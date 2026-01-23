using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Entities;
public class Lawyer : AppUser
{
    public string LicenseNumber { get; set; }= string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public DateTime JoinedDate { get; set; }
    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }
    public VerificationStatus? VerificationStatus { get; set; }
    public SessionType? SessionType { get; set; }
    public double? AverageRating { get; set; }
    public bool IsActive { get; set; }
}
