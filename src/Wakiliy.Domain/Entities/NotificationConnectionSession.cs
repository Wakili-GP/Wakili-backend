namespace Wakiliy.Domain.Entities;

public class NotificationConnectionSession
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser User { get; set; } = default!;
}
