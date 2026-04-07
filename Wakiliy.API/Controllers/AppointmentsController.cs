using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Common;
using Wakiliy.Application.Features.Appointments.Commands.Create;
using Wakiliy.Application.Features.Appointments.Commands.Delete;
using Wakiliy.Application.Features.Appointments.Commands.Update;
using Wakiliy.Application.Features.Appointments.Queries.GetByLawyer;
using System.Threading.Tasks;
using System.Security.Claims;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentSlotsController : ControllerBase
    {
        private protected readonly IMediator Mediator;

        public AppointmentSlotsController(IMediator mediator)
        {
            Mediator = mediator;
        }

        /// <summary>
        /// Get all appointment slots for a lawyer
        /// </summary>
        /// <response code="200">List of appointment slots returned</response>
        /// <response code="404">Lawyer not found</response>
        [HttpGet("lawyer/{lawyerId}")]
        [ProducesResponseType(typeof(List<AppointmentSlotDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLawyerAppointmentSlots(string lawyerId)
        {
            var query = new GetAppointmentSlotsByLawyerQuery { LawyerId = lawyerId };
            var result = await Mediator.Send(query);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }


        /// <summary>
        /// Get all my appointment slots for a lawyer
        /// </summary>
        /// <response code="200">List of appointment slots returned</response>
        [HttpGet()]
        [Authorize(Roles = $"{DefaultRoles.Lawyer}")]
        [ProducesResponseType(typeof(List<AppointmentSlotDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyAppointmentSlots()
        {
            var query = new GetAppointmentSlotsByLawyerQuery { LawyerId = User.GetUserId() };
            var result = await Mediator.Send(query);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Create a new appointment slot for the current lawyer
        /// </summary>
        /// <param name="dto">The creation payload</param>
        /// <response code="200">Appointment slot created</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="409">Slot overlaps with an existing slot</response>
        [HttpPost]
        [Authorize(Roles = DefaultRoles.Lawyer)]
        [ProducesResponseType(typeof(AppointmentSlotDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentSlotDto dto)
        {
            var command = new CreateAppointmentSlotCommand
            {
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                LawyerId = User.GetUserId()
            };
            var result = await Mediator.Send(command);
            return result.IsSuccess ? result.ToSuccess("Appointment slot created successfully") : result.ToProblem();
        }

        /// <summary>
        /// Update an existing appointment slot
        /// </summary>
        /// <param name="id">The slot ID</param>
        /// <param name="dto">The update payload</param>
        /// <response code="200">Appointment slot updated</response>
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
              DayOfWeek = dto.DayOfWeek,
              StartTime = dto.StartTime,
              EndTime = dto.EndTime,  
              Id = id,
              LawyerId = User.GetUserId()
            };

            var result = await Mediator.Send(command);
            return result.IsSuccess ? result.ToSuccess("Appointment slot updated successfully") : result.ToProblem();
        }

        /// <summary>
        /// Delete an appointment slot
        /// </summary>
        /// <param name="id">The slot ID</param>
        /// <response code="204">Appointment slot deleted</response>
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
