using Wakiliy.Domain.Enums;

namespace Wakiliy.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>The user who receives this notification.</summary>
    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }

    /// <summary>Optional reference ID (e.g. AppointmentId, ReviewId).</summary>
    public string? ReferenceId { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser User { get; set; } = default!;
}
