using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; }

    public int SlotId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string LawyerId { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public AppointmentSlot Slot { get; set; } = default!;

    public Client Client { get; set; } = default!;
    public Lawyer Lawyer { get; set; } = default!;
}
