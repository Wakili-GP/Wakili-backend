using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Forums.Commands.ChangePostStatus;
using Wakiliy.Application.Features.Forums.Commands.CreateComment;
using Wakiliy.Application.Features.Forums.Commands.CreatePost;
using Wakiliy.Application.Features.Forums.Commands.LikeComment;
using Wakiliy.Application.Features.Forums.Commands.LikePost;
using Wakiliy.Application.Features.Forums.Queries.GetAllPostsForAdmin;
using Wakiliy.Application.Features.Forums.Queries.GetApprovedPosts;
using Wakiliy.Application.Features.Forums.Queries.GetForumStats;
using Wakiliy.Application.Features.Forums.Queries.GetLatestPosts;
using Wakiliy.Application.Features.Forums.Queries.GetPostById;
using Wakiliy.Application.Features.Forums.Queries.GetPostComments;
using Wakiliy.Domain.Entities;
using Wakiliy.API.Extensions;

namespace Wakiliy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForumsController(ISender sender) : ControllerBase
{
    // ==========================================
    // Public Endpoints
    // ==========================================

    [HttpGet("posts")]
    public async Task<IActionResult> GetApprovedPosts([FromQuery] string? keyword, [FromQuery] int? specializationId, [FromQuery] string sortBy = "newest", [FromQuery] int page = 1, [FromQuery] int limit = 9)
    {
        var result = await sender.Send(new GetApprovedPostsQuery(keyword, specializationId, sortBy, page, limit));
        return result.IsSuccess ? Ok(new { success = true, data = result.Value }) : BadRequest(result.Error);
    }

    [HttpGet("posts/latest")]
    public async Task<IActionResult> GetLatestPosts([FromQuery] int limit = 6)
    {
        var result = await sender.Send(new GetLatestPostsQuery(limit));
        return result.IsSuccess ? Ok(new { success = true, data = result.Value }) : BadRequest(result.Error);
    }

    [HttpGet("posts/{id}")]
    public async Task<IActionResult> GetPostById(string id)
    {
        var result = await sender.Send(new GetPostByIdQuery(id));
        return result.IsSuccess ? Ok(new { success = true, data = result.Value }) : NotFound(result.Error);
    }

    [HttpGet("posts/{id}/comments")]
    public async Task<IActionResult> GetPostComments(string id)
    {
        var result = await sender.Send(new GetPostCommentsQuery(id));
        return result.IsSuccess ? Ok(new { success = true, data = result.Value }) : BadRequest(result.Error);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await sender.Send(new GetForumStatsQuery());
        return result.IsSuccess ? Ok(new { success = true, data = result.Value }) : BadRequest(result.Error);
    }

    // ==========================================
    // Authenticated User Endpoints
    // ==========================================

    [HttpPost("posts")]
    // [Authorize] // Uncomment when Auth is properly set up
    public async Task<IActionResult> CreatePost([FromBody] CreatePostCommand command)
    {
        var actualCommand = command with { AuthorId = User.GetUserId() };
        var result = await sender.Send(actualCommand);
        return result.IsSuccess ? Ok(new { success = true, data = result.Value }) : BadRequest(result.Error);
    }

    [HttpPost("posts/{id}/react")]
    // [Authorize]
    public async Task<IActionResult> ReactToPost(string id)
    {
        var result = await sender.Send(new LikePostCommand(id));
        return result.IsSuccess ? Ok(new { success = true }) : NotFound(result.Error);
    }

    [HttpPost("posts/{id}/comments")]
    // [Authorize]
    public async Task<IActionResult> CreateComment(string id, [FromBody] CreateCommentCommand command)
    {
        // Enforce PostId from URL and set AuthorId
        var actualCommand = command with { PostId = id, AuthorId = User.GetUserId() };
        var result = await sender.Send(actualCommand);
        return result.IsSuccess ? Ok(new { success = true, data = result.Value }) : BadRequest(result.Error);
    }

    [HttpPost("comments/{id}/react")]
    // [Authorize]
    public async Task<IActionResult> ReactToComment(string id)
    {
        var result = await sender.Send(new LikeCommentCommand(id));
        return result.IsSuccess ? Ok(new { success = true }) : NotFound(result.Error);
    }

    // ==========================================
    // Admin Endpoints
    // ==========================================

    [HttpGet("admin/posts")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllPostsForAdmin([FromQuery] string? keyword, [FromQuery] int? specializationId, [FromQuery] string sortBy = "newest", [FromQuery] int page = 1, [FromQuery] int limit = 9)
    {
        var result = await sender.Send(new GetAllPostsForAdminQuery(keyword, specializationId, sortBy, page, limit));
        return result.IsSuccess ? Ok(new { success = true, data = result.Value }) : BadRequest(result.Error);
    }

    [HttpPost("admin/posts/{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApprovePost(string id)
    {
        var result = await sender.Send(new ChangePostStatusCommand(id, PostStatus.Approved));
        return result.IsSuccess ? Ok(new { success = true }) : BadRequest(result.Error);
    }

    [HttpPost("admin/posts/{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectPost(string id)
    {
        var result = await sender.Send(new ChangePostStatusCommand(id, PostStatus.Rejected));
        return result.IsSuccess ? Ok(new { success = true }) : BadRequest(result.Error);
    }
}
