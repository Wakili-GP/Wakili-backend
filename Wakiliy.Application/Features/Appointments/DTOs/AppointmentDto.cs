using Wakiliy.Domain.Enums;

namespace Wakiliy.Application.Features.Appointments.DTOs;

public class AppointmentDto
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
    public string? LawyerFirstName { get; set; }
    public string? LawyerLastName { get; set; }
    public string? LawyerProfileImage { get; set; }
    public string? LawyerId { get; set; }
}

