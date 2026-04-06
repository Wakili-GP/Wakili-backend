using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Users.Commands.DeleteUser;
using Wakiliy.Application.Features.Users.Commands.ToggleUserStatus;
using Wakiliy.Application.Features.Users.Commands.UpdateUser;
using Wakiliy.Application.Features.Users.DTOs;
using Wakiliy.Application.Features.Users.Queries.GetUserById;
using Wakiliy.Application.Features.Users.Queries.GetUsers;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{DefaultRoles.Admin},{DefaultRoles.SuperAdmin}")]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Get all users (lawyers and clients) with pagination and filters
        /// </summary>
        /// <remarks>
        /// Returns a paginated list of all users based on query filters
        /// </remarks>
        /// <response code="200">Paginated list of users returned</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Admins only</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<UserListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
        {
            var result = await mediator.Send(query);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Get a user by ID
        /// </summary>
        /// <response code="200">User returned</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">User not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await mediator.Send(new GetUserByIdQuery(id));
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <remarks>
        /// Updates user properties (supports partial updates). Cannot update admin users.
        /// </remarks>
        /// <response code="200">User updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Cannot update admin users</response>
        /// <response code="404">User not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserCommand request)
        {
            request.Id = id;
            var result = await mediator.Send(request);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        /// <summary>
        /// Toggle status a user
        /// </summary>
        /// <remarks>
        /// Toggles the user's status between Active and Inactive. 
        /// </remarks>
        /// <response code="200">User status toggled successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">User not found</response>
        [HttpPut("toggle-status/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var result = await mediator.Send(new ToggleUserStatusCommand { Id = id });
            return result.IsSuccess ? result.ToSuccess("Status toggled successfully") : result.ToProblem();
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <remarks>
        /// Permanently removes a user from the system. Cannot delete admin users.
        /// </remarks>
        /// <response code="204">User deleted successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Cannot delete admin users</response>
        /// <response code="404">User not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await mediator.Send(new DeleteUserCommand { Id = id });
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
