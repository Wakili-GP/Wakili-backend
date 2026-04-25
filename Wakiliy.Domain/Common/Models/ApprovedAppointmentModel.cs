namespace Wakiliy.Domain.Common.Models;

public class ApprovedAppointmentModel
{
    public Guid AppointmentId { get; set; }
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string ClientFirstName { get; set; } = string.Empty;
    public string ClientLastName { get; set; } = string.Empty;
    public string ClientPhoneNumber { get; set; } = string.Empty;
    public string? ClientProfileImage { get; set; }
    public int AppointmentType { get; set; }
    public decimal SessionPrice { get; set; }
}
