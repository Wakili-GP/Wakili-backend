using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveBasicInfo;
using Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveEducation;
using Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveExperience;
using Wakiliy.Application.Features.Lawyers.Onboarding.Commands.SaveVerification;
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
    public async Task<IActionResult> SaveBasicInfo([FromBody] SaveBasicInfoCommand command, CancellationToken cancellationToken)
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
    public async Task<IActionResult> SaveEducation([FromBody] SaveEducationCommand command, CancellationToken cancellationToken)
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
    public async Task<IActionResult> SaveVerification([FromBody] SaveVerificationCommand command, CancellationToken cancellationToken)
    {
        command.UserId = User.GetUserId();
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }
}
