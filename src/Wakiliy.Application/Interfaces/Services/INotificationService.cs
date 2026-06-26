using Wakiliy.Domain.Enums;

namespace Wakiliy.Application.Interfaces.Services;

public interface INotificationService
{
    /// <summary>
    /// Persists a notification in the database and pushes it in real-time
    /// to the target user via SignalR if they are connected.
    /// </summary>
    Task SendNotificationAsync(
        string userId,
        string title,
        string message,
        NotificationType type,
        string? referenceId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends the same notification to multiple users at once.
    /// </summary>
    Task SendNotificationToManyAsync(
        IEnumerable<string> userIds,
        string title,
        string message,
        NotificationType type,
        string? referenceId = null,
        CancellationToken cancellationToken = default);
}
