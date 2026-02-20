using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Admins.Commands.CreateAdmin;
using Wakiliy.Application.Features.Admins.Commands.DeleteAdmin;
using Wakiliy.Application.Features.Admins.Commands.UpdateAdmin;
using Wakiliy.Application.Features.Admins.DTOs;
using Wakiliy.Application.Features.Admins.Queries.GetAdmins;
using Wakiliy.Application.Features.Auth.Commands.Register;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = DefaultRoles.Admin)]
    public class AdminsController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Get all administrators
        /// </summary>
        /// <remarks>
        /// Returns list of users who have admin role
        /// </remarks>
        /// <response code="200">List of admins returned</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Only super admin allowed</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<AdminDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAdmins()
        {
            var result = await mediator.Send(new GetAdminsQuery());
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Create a new administrator
        /// </summary>
        /// <remarks>
        /// Creates a new user with admin role
        /// </remarks>
        /// <response code="201">Admin created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="409">Email already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(AdminDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminCommand request)
        {
            var result = await mediator.Send(request);
            return result.IsSuccess 
                ? CreatedAtAction(nameof(GetAdmins), new { id = result.Value?.Id }, result.Value) 
                : result.ToProblem();
        }

        /// <summary>
        /// Update an administrator
        /// </summary>
        /// <remarks>
        /// Updates admin properties (supports partial updates)
        /// </remarks>
        /// <response code="200">Admin updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Admin not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AdminDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAdmin(string id, [FromBody] UpdateAdminRequest request)
        {
            var command = new UpdateAdminCommand
            {
                Id = id,
                Status = request.Status,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await mediator.Send(command);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Delete an administrator
        /// </summary>
        /// <remarks>
        /// Removes an admin user from the system
        /// </remarks>
        /// <response code="204">Admin deleted successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Admin not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var command = new DeleteAdminCommand { Id = id };
            var result = await mediator.Send(command);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
