using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Reviews.Commands.Create;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Application.Features.Reviews.Queries.GetAll;
using Wakiliy.Application.Features.Reviews.Queries.GetAllSystemReviews;
using Wakiliy.Application.Features.Reviews.Queries.GetByLawyer;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers;

[Route("api/reviews")]
[ApiController]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a lawyer review (and optionally a system review) after a completed appointment.
    /// </summary>
    /// <param name="dto">The review creation payload</param>
    /// <response code="200">Review created successfully</response>
    /// <response code="400">Appointment not completed or invalid input</response>
    /// <response code="404">Appointment not found</response>
    /// <response code="409">Review already exists for this appointment</response>
    [HttpPost]
    [Authorize(Roles = DefaultRoles.Client)]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequestDto dto)
    {
        var command = dto.Adapt<CreateReviewCommand>();
        command.UserId = User.GetUserId();

        var result = await _mediator.Send(command);
        return result.IsSuccess ? result.ToSuccess("Review created successfully") : result.ToProblem();
    }

    /// <summary>
    /// Get all lawyer reviews (Admin only).
    /// </summary>
    /// <response code="200">List of all reviews</response>
    [HttpGet("admin")]
    [Authorize(Roles = $"{DefaultRoles.Admin},{DefaultRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(List<ReviewResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllReviews()
    {
        var result = await _mediator.Send(new GetAllReviewsQuery());
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    /// <summary>
    /// Get all system/platform reviews (Admin only).
    /// </summary>
    /// <response code="200">List of all system reviews</response>
    [HttpGet("system-reviews/admin")]
    [Authorize(Roles = $"{DefaultRoles.Admin},{DefaultRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(List<SystemReviewResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSystemReviews()
    {
        var result = await _mediator.Send(new GetAllSystemReviewsQuery());
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    /// <summary>
    /// Get all reviews for a specific lawyer (without flagged AI reviews).
    /// </summary>
    /// <param name="lawyerId">The lawyer's ID</param>
    /// <response code="200">List of reviews for the lawyer</response>
    [HttpGet("lawyer/{lawyerId}")]
    [ProducesResponseType(typeof(List<ReviewResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLawyerId(string lawyerId)
    {
        var query = new GetReviewsByLawyerIdQuery { LawyerId = lawyerId };
        var result = await _mediator.Send(query);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }
}
