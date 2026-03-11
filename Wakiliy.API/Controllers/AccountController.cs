using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Account.Commands.UpdateClientInfo;
using Wakiliy.Application.Features.Account.Commands.UpdateLawyerInfo;
using Wakiliy.Domain.Constants;
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
        /// Update the current client's profile.
        /// </summary>
        /// <param name="command">Profile fields to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated user profile.</returns>
        [HttpPut("client-info")]
        [Authorize(Roles = DefaultRoles.Client)]
        [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateClientInfo([FromBody] UpdateClientInfoCommand command, CancellationToken cancellationToken)
        {
            command.Id = User.GetUserId();
            var result = await mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        /// <summary>
        /// Update the current lawyer's profile.
        /// </summary>
        /// <param name="command">Profile fields to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated user profile.</returns>
        [HttpPut("lawyer-info")]
        [Authorize(Roles = DefaultRoles.Lawyer)]
        [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLawyerInfo([FromBody] UpdateLawyerInfoCommand command, CancellationToken cancellationToken)
        {
            command.Id = User.GetUserId();
            var result = await mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }
}