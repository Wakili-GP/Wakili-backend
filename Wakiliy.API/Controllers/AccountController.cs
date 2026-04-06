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
                Address = request.Address,
                Gender = request.Gender
            };

            var result = await mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
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
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                ProfileImage = request.ProfileImage,
                Gender = request.Gender,
                Address = request.Address,
                LicenseNumber = request.LicenseNumber,
                SpecializationIds = request.SpecializationIds,
                YearsOfExperience = request.YearsOfExperience,
                PhoneSessionPrice = request.PhoneSessionPrice,
                InOfficeSessionPrice = request.InOfficeSessionPrice
            };

            var result = await mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }
}