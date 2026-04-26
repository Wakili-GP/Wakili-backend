using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.Application.Features.AppointmentSlots.Commands.Create;
using Wakiliy.Application.Features.AppointmentSlots.Commands.Delete;
using Wakiliy.Application.Features.AppointmentSlots.Commands.Update;
using Wakiliy.Application.Features.AppointmentSlots.Queries.GetByLawyer;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.AppointmentSlots.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Enums;

namespace Wakiliy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase
    {
        private protected readonly IMediator Mediator;

        public TimeSlotsController(IMediator mediator)
        {
            Mediator = mediator;
        }

        /// <summary>
        /// Get all time slots for a lawyer
        /// </summary>
        /// <response code="200">List of time slots returned</response>
        /// <response code="404">Lawyer not found</response>
        [HttpGet("lawyer/{lawyerId}")]
        [ProducesResponseType(typeof(List<AppointmentSlotDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLawyerTimeSlots(string lawyerId, [FromQuery] DateOnly? date, [FromQuery] SessionType? sessionType)
        {
            var query = new GetAppointmentSlotsByLawyerQuery { LawyerId = lawyerId, Date = date, SessionType = sessionType };
            var result = await Mediator.Send(query);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Get all my time slots for a lawyer
        /// </summary>
        /// <response code="200">List of time slots returned</response>
        [HttpGet()]
        [Authorize(Roles = $"{DefaultRoles.Lawyer}")]
        [ProducesResponseType(typeof(List<AppointmentSlotDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTimeSlots([FromQuery] DateOnly date)
        {
            var query = new GetAppointmentSlotsByLawyerQuery { LawyerId = User.GetUserId(), Date = date };
            var result = await Mediator.Send(query);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Create a new time slot for the current lawyer
        /// </summary>
        /// <param name="dto">The creation payload</param>
        /// <response code="200">Time slot created</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="409">Slot overlaps with an existing slot</response>
        [HttpPost]
        [Authorize(Roles = DefaultRoles.Lawyer)]
        [ProducesResponseType(typeof(AppointmentSlotDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentSlotDto dto)
        {
            var command = new CreateAppointmentSlotCommand
            {
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                SessionType = dto.SessionType,
                LawyerId = User.GetUserId()
            };
            var result = await Mediator.Send(command);
            return result.IsSuccess ? result.ToSuccess("Appointment slot created successfully") : result.ToProblem();
        }

        /// <summary>
        /// Update an existing time slot
        /// </summary>
        /// <param name="id">The slot ID</param>
        /// <param name="dto">The update payload</param>
        /// <response code="200">Time slot updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Slot not found</response>
        /// <response code="409">Slot overlaps with an existing slot</response>
        [HttpPut("{id}")]
        [Authorize(Roles = DefaultRoles.Lawyer)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentSlotDto dto)
        {       
            var command = new UpdateAppointmentSlotCommand
            {
              Date = dto.Date,
              StartTime = dto.StartTime,
              EndTime = dto.EndTime,  
              SessionType = dto.SessionType,
              Id = id,
              LawyerId = User.GetUserId()
            };

            var result = await Mediator.Send(command);
            return result.IsSuccess ? result.ToSuccess("Appointment slot updated successfully") : result.ToProblem();
        }

        /// <summary>
        /// Delete an time slot
        /// </summary>
        /// <param name="id">The slot ID</param>
        /// <response code="204">Time slot deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Slot not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{DefaultRoles.Lawyer},{DefaultRoles.Admin}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Mediator.Send(new DeleteAppointmentSlotCommand { Id = id });
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
