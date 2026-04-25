using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Entities;

public class BookingIntent
{
    public Guid Id { get; set; }
    public int SlotId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string LawyerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public BookingIntentStatus Status { get; set; } = BookingIntentStatus.Pending;
    public string? PaymobOrderId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    public AppointmentSlot Slot { get; set; } = default!;
    public Client Client { get; set; } = default!;
    public Lawyer Lawyer { get; set; } = default!;
}