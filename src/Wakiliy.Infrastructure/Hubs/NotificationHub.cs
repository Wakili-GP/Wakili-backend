using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Wakiliy.Application.Common.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Hubs;

[Authorize]
public class NotificationHub(
    ApplicationDbContext dbContext,
    ILogger<NotificationHub> logger) : Hub
{
    // ──────────────────────────────────────────────────────────────────
    // Connection lifecycle
    // ──────────────────────────────────────────────────────────────────

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        // Track the connection session
        dbContext.NotificationConnectionSessions.Add(new NotificationConnectionSession
        {
            UserId = userId,
            ConnectionId = Context.ConnectionId,
            ConnectedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        logger.LogInformation("NotificationHub connected: UserId={UserId}, ConnectionId={ConnectionId}", userId, Context.ConnectionId);

        // Push all unread notifications to the freshly connected user
        var unread = await dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                ReferenceId = n.ReferenceId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();

        if (unread.Count > 0)
            await Clients.Caller.SendAsync("UnreadNotifications", unread);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;

        var session = await dbContext.NotificationConnectionSessions
            .FirstOrDefaultAsync(s => s.ConnectionId == connectionId);

        if (session is not null)
        {
            dbContext.NotificationConnectionSessions.Remove(session);
            await dbContext.SaveChangesAsync();
        }

        logger.LogInformation("NotificationHub disconnected: ConnectionId={ConnectionId}", connectionId);

        await base.OnDisconnectedAsync(exception);
    }

    // ──────────────────────────────────────────────────────────────────
    // Client-callable methods
    // ──────────────────────────────────────────────────────────────────

    /// <summary>Mark a single notification as read.</summary>
    public async Task MarkAsRead(Guid notificationId)
    {
        var userId = GetUserId();

        var notification = await dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification is null)
            return;

        notification.IsRead = true;
        await dbContext.SaveChangesAsync();

        await Clients.Caller.SendAsync("NotificationRead", notificationId);
    }

    /// <summary>Mark all unread notifications for the caller as read.</summary>
    public async Task MarkAllAsRead()
    {
        var userId = GetUserId();

        var unread = await dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var n in unread)
            n.IsRead = true;

        await dbContext.SaveChangesAsync();

        await Clients.Caller.SendAsync("AllNotificationsRead");
    }

    // ──────────────────────────────────────────────────────────────────
    private string GetUserId() =>
        Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("UserId missing from token.");
}
