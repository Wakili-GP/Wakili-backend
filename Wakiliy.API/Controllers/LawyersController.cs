using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Lawyers.Commands.Create;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = DefaultRoles.Admin)]
    public class LawyersController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> GetLawyers([FromBody] CreateLaywerCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
    }
}
