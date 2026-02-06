using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Account.Commands.Update;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Application.Features.Account.Queries.GetInfo;

namespace Wakiliy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController(IMediator mediator) : ControllerBase
    {

        /// <summary>
        /// Update the current user's profile.
        /// </summary>
        /// <param name="command">Profile fields to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated user profile.</returns>
        [HttpPut("info")]
        [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateInfo([FromBody] UpdateAccountCommand command, CancellationToken cancellationToken)
        {
            command.Id = User.GetUserId();
            var result = await mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }
}