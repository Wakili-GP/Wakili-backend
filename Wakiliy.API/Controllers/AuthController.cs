using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Auth.Commands.ConfirmEmail;
using Wakiliy.Application.Features.Auth.Commands.Register;

namespace Wakiliy.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command,cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody]ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}
