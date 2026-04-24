using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Common.Models;

public class LawyerReceivedAppointmentModel
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

public class AppointmentsReceivedStats
{
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Confirmed { get; set; }
    public int Cancelled { get; set; }
    public int Completed { get; set; }
}