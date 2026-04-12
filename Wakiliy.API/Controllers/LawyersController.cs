using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Lawyers.Commands.Create;
using Wakiliy.Application.Features.Lawyers.Commands.Delete;
using Wakiliy.Application.Features.Lawyers.Commands.ToggleStatus;
using Wakiliy.Application.Features.Lawyers.Commands.Update;
using Wakiliy.Application.Features.Lawyers.Commands.Verification.ApproveVerification;
using Wakiliy.Application.Features.Lawyers.Commands.Verification.RejectVerification;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Application.Features.Lawyers.Queries.GetAll;
using Wakiliy.Application.Features.Lawyers.Queries.GetApprovedLawyers;
using Wakiliy.Application.Features.Lawyers.Queries.GetById;
using Wakiliy.Application.Features.Lawyers.Queries.GetVerificationRequestById;
using Wakiliy.Application.Features.Lawyers.Queries.GetVerificationRequests;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Enums;

namespace Wakiliy.API.Controllers
{
    /// <summary>
    /// Manage lawyers in the system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{DefaultRoles.Admin},{DefaultRoles.SuperAdmin}")]
    public class LawyersController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Get lawyer verification requests with optional status filters.
        /// </summary>
        /// <param name="status">Verification status filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of verification requests.</returns>
        [HttpGet("lawyer-verification")]
        [ProducesResponseType(typeof(List<LawyerVerificationRequestResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLawyerVerificationRequests([FromQuery] VerificationStatus? status, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetLawyerVerificationRequestsQuery(status), cancellationToken);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Get a single lawyer verification request by id with full details.
        /// </summary>
        /// <param name="id">Lawyer id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The detailed verification request.</returns>
        [HttpGet("lawyer-verification/{id}")]
        [ProducesResponseType(typeof(LawyerVerificationDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLawyerVerificationRequestById(string id, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetLawyerVerificationRequestByIdQuery(id), cancellationToken);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Approve a lawyer's verification
        /// </summary>
        /// <remarks>
        /// Approves the verification request submitted by a lawyer
        /// </remarks>
        /// <response code="200">Verification approved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Lawyer not found</response>
        [HttpPut("verify/approve/{lawyerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveVerification(string lawyerId)
        {
            var command = new ApproveVerificationCommand { LawyerId = lawyerId, AdminId = User.GetUserId() };
            var result = await mediator.Send(command);
            return result.IsSuccess ? result.ToSuccess("Verification approved successfully") : result.ToProblem();
        }

        /// <summary>
        /// Reject a lawyer's verification
        /// </summary>
        /// <remarks>
        /// Rejects the verification request submitted by a lawyer, with an optional note
        /// </remarks>
        /// <response code="200">Verification rejected successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Lawyer not found</response>
        [HttpPut("verify/reject/{lawyerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectVerification(string lawyerId, [FromBody] RejectVerificationRequest request)
        {
            var command = new RejectVerificationCommand { LawyerId = lawyerId, Note = request.Note, AdminId = User.GetUserId() };
            var result = await mediator.Send(command);
            return result.IsSuccess ? result.ToSuccess("Verification rejected successfully") : result.ToProblem();
        }

        /// <summary>
        /// Get all approved lawyers available to all users
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of approved and active lawyers.</returns>
        /// <response code="200">Paginated list of approved lawyers returned</response>
        [HttpGet("approved")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PaginatedResult<LawyerResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApprovedLawyers([FromQuery] GetApprovedLawyersQuery request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }
    }
    
}
