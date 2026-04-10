using Wakiliy.Domain.Enums;

namespace Wakiliy.Application.Features.Appointments.DTOs;

public class LawyerReceivedAppointmentDto
{
    public Guid Id { get; set; }
    public AppointmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateOnly SessionDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public SessionType SessionType { get; set; }

    // Client info
    public string ClientId { get; set; } = string.Empty;
    public string? ClientFirstName { get; set; }
    public string? ClientLastName { get; set; }
    public string? ClientProfileImage { get; set; }
    public string? ClientPhone { get; set; }
}
