using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Lawyers.Commands.Create;
using Wakiliy.Application.Features.Lawyers.Commands.Delete;
using Wakiliy.Application.Features.Lawyers.Commands.ToggleStatus;
using Wakiliy.Application.Features.Lawyers.Commands.Update;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Application.Features.Lawyers.Queries.GetAll;
using Wakiliy.Application.Features.Lawyers.Queries.GetById;
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
    [Authorize(Roles = DefaultRoles.Admin)]
    public class LawyersController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Create a new lawyer.
        /// </summary>
        /// <param name="command">Data to create the lawyer.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Created lawyer info.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(LawyerResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateLawyer([FromBody] CreateLaywerCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(GetLawyerById), new { id = result.Value.Id }, result.Value) : result.ToProblem();
        }

        /// <summary>
        /// Get all lawyers.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of lawyers.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<LawyerResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLawyers(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllLawyersQuery(), cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

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
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        /// <summary>
        /// Get a lawyer by id.
        /// </summary>
        /// <param name="id">Lawyer id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The lawyer details.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LawyerDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLawyerById(string id, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetLawyerByIdQuery(id), cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        /// <summary>
        /// Update a lawyer by id.
        /// </summary>
        /// <param name="id">Lawyer id.</param>
        /// <param name="command">Fields to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated lawyer.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LawyerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLawyer(string id, [FromBody] UpdateLawyerCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        /// <summary>
        /// Toggle lawyer active status.
        /// </summary>
        /// <param name="id">Lawyer id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>OK if toggled.</returns>
        [HttpPut("{id}/toggle-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleStatus(string id, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new ToggleLawyerActiveStatusCommand(id), cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        /// <summary>
        /// Delete a lawyer by id.
        /// </summary>
        /// <param name="id">Lawyer id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLawyer(string id, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new DeleteLawyerCommand(id), cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
