using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Application.Features.Account.Queries.GetInfo;
using Wakiliy.Application.Features.Auth.Commands.ConfirmEmail;
using Wakiliy.Application.Features.Auth.Commands.ForgotPassword;
using Wakiliy.Application.Features.Auth.Commands.Login;
using Wakiliy.Application.Features.Auth.Commands.Register;
using Wakiliy.Application.Features.Auth.Commands.ResendConfirmEmail;
using Wakiliy.Application.Features.Auth.Commands.ResetPassword;
using Wakiliy.Application.Features.Auth.DTOs;

namespace Wakiliy.API.Controllers;

/// <summary>
/// Authentication endpoints (register, confirm email, login).
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="command">Registration data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>OK if registered.</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Created() : result.ToProblem();
    }

    /// <summary>
    /// Confirm user email.
    /// </summary>
    /// <param name="command">Confirmation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>OK if confirmed.</returns>
    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    /// <summary>
    /// Resend Confirm user email.
    /// </summary>
    /// <param name="command">Confirmation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>OK if confirmed.</returns>
    [HttpPost("resend-verification")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Login and get a token.
    /// </summary>
    /// <param name="command">Login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>JWT token and user info.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    /// <summary>
    /// Forget password and send code to email.
    /// </summary>
    /// <param name="command">Email for password reset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>OK if request was successful.</returns>
    [HttpPost("forget-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
    }


    /// <summary>
    /// Reset user password using OTP.
    /// </summary>
    /// <param name="command">Email for password reset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>OK if request was successful.</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
    }


    /// <summary>
    /// Get information about the current logged-in user.
    /// </summary>
    /// <returns>User profile info.</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInfo(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAccountInfoQuery(User.GetUserId()), cancellationToken);

        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

}
