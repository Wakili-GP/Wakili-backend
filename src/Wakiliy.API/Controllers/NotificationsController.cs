using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Wakiliy.Application.Common.DTOs;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.API.Controllers;

/// <summary>
/// Provides REST endpoints for reading and marking notifications.
/// Real-time delivery is handled via the SignalR hub at /hubs/notifications.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController(ApplicationDbContext dbContext) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    /// <summary>Get paginated notifications for the authenticated user (unread first).</summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page (default 20).</param>
    /// <response code="200">List of notifications.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var notifications = await dbContext.Notifications
            .Where(n => n.UserId == UserId)
            .OrderBy(n => n.IsRead)           // unread first
            .ThenByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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

        return Ok(notifications);
    }

    /// <summary>Get the count of unread notifications for the authenticated user.</summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var count = await dbContext.Notifications
            .CountAsync(n => n.UserId == UserId && !n.IsRead);

        return Ok(count);
    }

    /// <summary>Mark a single notification as read.</summary>
    /// <param name="id">The notification ID.</param>
    /// <response code="204">Marked as read.</response>
    /// <response code="404">Notification not found.</response>
    [HttpPut("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var notification = await dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == UserId);

        if (notification is null)
            return NotFound();

        notification.IsRead = true;
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>Mark all notifications for the authenticated user as read.</summary>
    /// <response code="204">All marked as read.</response>
    [HttpPut("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var unread = await dbContext.Notifications
            .Where(n => n.UserId == UserId && !n.IsRead)
            .ToListAsync();

        foreach (var n in unread)
            n.IsRead = true;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
