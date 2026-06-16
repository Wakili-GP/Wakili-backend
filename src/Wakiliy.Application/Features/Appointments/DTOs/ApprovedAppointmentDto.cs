namespace Wakiliy.Application.Features.Appointments.DTOs;

public class ApprovedAppointmentDto
{
    public string AppointmentId { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string ClientFirstName { get; set; } = string.Empty;
    public string ClientLastName { get; set; } = string.Empty;
    public string ClientPhoneNumber { get; set; } = string.Empty;
    public string? ClientProfileImage { get; set; }
    public int AppointmentType { get; set; }
    public decimal SessionPrice { get; set; }
}
