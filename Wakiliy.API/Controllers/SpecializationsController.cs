using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Specializations.Commands.Create;
using Wakiliy.Application.Features.Specializations.Commands.Delete;
using Wakiliy.Application.Features.Specializations.Commands.Update;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Application.Features.Specializations.Queries.GetActive;
using Wakiliy.Application.Features.Specializations.Queries.GetAll;
using Wakiliy.Application.Features.Specializations.Queries.GetById;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Admin)]
public class SpecializationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<SpecializationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllSpecializationsQuery(), cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SpecializationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSpecializationByIdQuery(id), cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    [HttpPost]
    [ProducesResponseType(typeof(SpecializationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateSpecializationCommand command, CancellationToken cancellationToken)
    {
        command.UserId = User.GetUserId();
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SpecializationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSpecializationCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        command.UserId = User.GetUserId();
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteSpecializationCommand { Id = id }, cancellationToken);
        return result.IsSuccess ? result.ToSuccess("Specialization deleted successfully") : result.ToProblem();
    }

    [HttpGet("active")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<SpecializationOptionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetActiveSpecializationsQuery(), cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }
}
