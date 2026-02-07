using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveBasicInfo;
using Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveEducation;
using Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveExperience;
using Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveVerification;
using Wakiliy.Application.Features.Lawyers.Queries.GetOnboardingProgress;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers;

[Route("api/lawyer/onboarding")]
[ApiController]
[Authorize(Roles = DefaultRoles.Lawyer)]
public class LawyerOnboardingController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Save Step 1 - Basic information for the current lawyer.
    /// </summary>
    [HttpPost("basic-info")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveBasicInfo([FromForm] SaveBasicInfoCommand command, CancellationToken cancellationToken)
    {
        command.UserId = User.GetUserId();
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    /// <summary>
    /// Save Step 2 - Education info for the current lawyer.
    /// </summary>
    [HttpPost("education")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveEducation([FromForm] SaveEducationCommand command, CancellationToken cancellationToken)
    {
        command.UserId = User.GetUserId();
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    /// <summary>
    /// Save Step 3 - Experience info for the current lawyer.
    /// </summary>
    [HttpPost("experience")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveExperience([FromBody] SaveExperienceCommand command, CancellationToken cancellationToken)
    {
        command.UserId = User.GetUserId();
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    /// <summary>
    /// Save Step 4 - Verification documents for the current lawyer.
    /// </summary>
    [HttpPost("verification")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveVerification([FromForm] SaveVerificationCommand command, CancellationToken cancellationToken)
    {
        command.UserId = User.GetUserId();
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    /// <summary>
    /// Gets the lawyer's current onboarding progress and all submitted data.
    /// </summary>
    [HttpGet("progress")]
    public async Task<IActionResult> GetProgress(CancellationToken cancellationToken)
    {
        var query = new GetOnboardingProgressQuery(User.GetUserId());
        var result = await mediator.Send(query, cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }
}
