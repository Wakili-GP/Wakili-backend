using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Account.Commands.ChangePassword;
using Wakiliy.Application.Features.Account.Commands.UpdateClientInfo;
using Wakiliy.Application.Features.Account.Commands.UpdateLawyerInfo;
using Wakiliy.Application.Features.Account.Queries.GetClientData;
using Wakiliy.Application.Features.Account.Queries.GetLawyerData;
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
        /// Get the current client's data.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Client data details.</returns>
        [HttpGet("client-info")]
        [Authorize(Roles = DefaultRoles.Client)]
        [ProducesResponseType(typeof(ClientDataDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClientData(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetClientDataQuery(User.GetUserId()), cancellationToken);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Get the current lawyer's data.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Lawyer data details.</returns>
        [HttpGet("lawyer-info")]
        [Authorize(Roles = DefaultRoles.Lawyer)]
        [ProducesResponseType(typeof(LawyerDataDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLawyerData(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetLawyerDataQuery(User.GetUserId()), cancellationToken);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Change the current user's password.
        /// </summary>
        /// <param name="request">The current and new passwords.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Success message.</returns>
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request, CancellationToken cancellationToken)
        {
            var command = new ChangePasswordCommand
            {
                UserId = User.GetUserId(),
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            };

            var result = await mediator.Send(command, cancellationToken);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Update the current client's profile.
        /// </summary>
        /// <param name="request">Profile fields to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated user profile.</returns>
        [HttpPut("client-info")]
        [Authorize(Roles = DefaultRoles.Client)]
        [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateClientInfo([FromForm] UpdateClientInfoDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateClientInfoCommand{
                Id = User.GetUserId(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                ProfileImage = request.ProfileImage,
                City = request.City,
                Country = request.Country,
                Bio = request.Bio
            };

            var result = await mediator.Send(command, cancellationToken);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Update the current lawyer's profile.
        /// </summary>
        /// <param name="request">Profile fields to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated user profile.</returns>
        [HttpPut("lawyer-info")]
        [Authorize(Roles = DefaultRoles.Lawyer)]
        [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLawyerInfo([FromForm] UpdateLawyerInfoDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateLawyerInfoCommand
            {
                Id = User.GetUserId(),
                PhoneNumber = request.PhoneNumber,
                ProfileImage = request.ProfileImage,
                City = request.City,
                Country = request.Country,
                Bio = request.Bio,
                Summary = request.Summary,
                PhoneSessionPrice = request.PhoneSessionPrice,
                InOfficeSessionPrice = request.InOfficeSessionPrice
            };

            var result = await mediator.Send(command, cancellationToken);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }
    }
}