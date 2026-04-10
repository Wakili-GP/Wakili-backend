using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Appointments.Commands.Complete;
using Wakiliy.Application.Features.Appointments.Commands.Confirm;
using Wakiliy.Application.Features.Appointments.Commands.Create;
using Wakiliy.Application.Features.Appointments.Commands.Reject;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Application.Features.Appointments.Queries.GetByClient;
using Wakiliy.Application.Features.Appointments.Queries.GetByLawyer;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private protected readonly IMediator Mediator;

        public AppointmentsController(IMediator mediator)
        {
            Mediator = mediator;
        }

        /// <summary>
        /// Create a new appointment By Client.
        /// </summary>
        /// <param name="dto">The appointment creation payload</param>
        /// <response code="200">Appointment created successfully</response>
        /// <response code="404">Lawyer or slot not found</response>
        /// <response code="409">Slot is already booked</response>
        [HttpPost]
        [Authorize(Roles = DefaultRoles.Client)]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            var command = new CreateAppointmentCommand
            {
                ClientId = User.GetUserId(),
                LawyerId = dto.LawyerId,
                SlotId = dto.SlotId,
                SessionType = dto.SessionType
            };

            var result = await Mediator.Send(command);
            return result.IsSuccess ? result.ToSuccess("Appointment created successfully") : result.ToProblem();
        }

        /// <summary>
        /// Get all appointments for the current client.
        /// </summary>
        /// <response code="200">List of client appointments</response>
        [HttpGet("my")]
        [Authorize(Roles = DefaultRoles.Client)]
        [ProducesResponseType(typeof(List<AppointmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyAppointments()
        {
            var query = new GetAppointmentsByClientQuery { ClientId = User.GetUserId() };
            var result = await Mediator.Send(query);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Get all received appointments for the current lawyer.
        /// </summary>
        /// <response code="200">List of lawyer's received appointments</response>
        [HttpGet("received")]
        [Authorize(Roles = DefaultRoles.Lawyer)]
        [ProducesResponseType(typeof(List<LawyerReceivedAppointmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReceivedAppointments()
        {
            var query = new GetAppointmentsByLawyerQuery { LawyerId = User.GetUserId() };
            var result = await Mediator.Send(query);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Confirm a pending appointment.
        /// </summary>
        /// <param name="id">The appointment ID</param>
        /// <response code="200">Appointment confirmed</response>
        /// <response code="400">Invalid status transition</response>
        /// <response code="403">Not authorized</response>
        /// <response code="404">Appointment not found</response>
        [HttpPut("{id}/confirm")]
        [Authorize(Roles = DefaultRoles.Lawyer)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var command = new ConfirmAppointmentCommand
            {
                AppointmentId = id,
                LawyerId = User.GetUserId()
            };

            var result = await Mediator.Send(command);
            return result.IsSuccess ? result.ToSuccess("Appointment confirmed successfully") : result.ToProblem();
        }

        /// <summary>
        /// Reject an appointment.
        /// </summary>
        /// <param name="id">The appointment ID</param>
        /// <response code="200">Appointment rejected</response>
        /// <response code="400">Invalid status transition</response>
        /// <response code="403">Not authorized</response>
        /// <response code="404">Appointment not found</response>
        [HttpPut("{id}/reject")]
        [Authorize(Roles = DefaultRoles.Lawyer)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reject(Guid id)
        {
            var command = new RejectAppointmentCommand
            {
                AppointmentId = id,
                LawyerId = User.GetUserId()
            };

            var result = await Mediator.Send(command);
            return result.IsSuccess ? result.ToSuccess("Appointment rejected successfully") : result.ToProblem();
        }

        /// <summary>
        /// Complete a confirmed appointment.
        /// </summary>
        /// <param name="id">The appointment ID</param>
        /// <response code="200">Appointment completed</response>
        /// <response code="400">Invalid status transition</response>
        /// <response code="403">Not authorized</response>
        /// <response code="404">Appointment not found</response>
        [HttpPut("{id}/complete")]
        [Authorize(Roles = DefaultRoles.Lawyer)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Complete(Guid id)
        {
            var command = new CompleteAppointmentCommand
            {
                AppointmentId = id,
                LawyerId = User.GetUserId()
            };

            var result = await Mediator.Send(command);
            return result.IsSuccess ? result.ToSuccess("Appointment completed successfully") : result.ToProblem();
        }
    }
}

