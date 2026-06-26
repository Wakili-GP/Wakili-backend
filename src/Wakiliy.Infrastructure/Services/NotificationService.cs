using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Wakiliy.Application.Common.DTOs;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Infrastructure.Data;
using Wakiliy.Infrastructure.Hubs;

namespace Wakiliy.Infrastructure.Services;

public class NotificationService(
    ApplicationDbContext dbContext,
    IHubContext<NotificationHub> hubContext,
    ILogger<NotificationService> logger) : INotificationService
{
    public async Task SendNotificationAsync(
        string userId,
        string title,
        string message,
        NotificationType type,
        string? referenceId = null,
        CancellationToken cancellationToken = default)
    {
        var notification = await PersistNotificationAsync(userId, title, message, type, referenceId, cancellationToken);

        try
        {
            await hubContext.Clients
                .User(userId)
                .SendAsync("ReceiveNotification", ToDto(notification), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not push notification to user {UserId} via SignalR (user may be offline).", userId);
        }
    }

    public async Task SendNotificationToManyAsync(
        IEnumerable<string> userIds,
        string title,
        string message,
        NotificationType type,
        string? referenceId = null,
        CancellationToken cancellationToken = default)
    {
        var ids = userIds.ToList();

        logger.LogInformation("Sending notification to {Count} users", ids.Count);
        foreach (var userId in ids)
        {
            logger.LogInformation("Sending notification to user {UserId}", userId);
            await PersistNotificationAsync(userId, title, message, type, referenceId, cancellationToken);
        }

        try
        {
            await hubContext.Clients
                .Users(ids)
                .SendAsync("ReceiveNotification", new NotificationDto
                {
                    Title = title,
                    Message = message,
                    Type = type,
                    ReferenceId = referenceId,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not push notification to users via SignalR.");
        }
    }

    // ──────────────────────────────────────────────────────────────────
    private async Task<Notification> PersistNotificationAsync(
        string userId,
        string title,
        string message,
        NotificationType type,
        string? referenceId,
        CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            ReferenceId = referenceId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Notifications.Add(notification);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Notification persisted: Id={Id}, UserId={UserId}, Type={Type}", notification.Id, userId, type);

        return notification;
    }

    private static NotificationDto ToDto(Notification n) => new()
    {
        Id = n.Id,
        Title = n.Title,
        Message = n.Message,
        Type = n.Type,
        ReferenceId = n.ReferenceId,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}
