using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Earnings.Queries.GetAll;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Admin)]
public class EarningsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllEarnings([FromQuery] GetAllEarningsQuery query)
    {
        var result = await mediator.Send(query);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }
}
