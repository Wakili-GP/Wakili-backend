namespace Wakiliy.Domain.Entities;
public class Lawyer : AppUser
{
    public string LicenseNumber { get; set; }= string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public DateTime JoinedDate { get; set; }
}
