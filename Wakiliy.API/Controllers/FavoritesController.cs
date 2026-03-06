using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.Lawyers.Commands.AddFavorite;
using Wakiliy.Application.Features.Lawyers.Commands.RemoveFavorite;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Application.Features.Lawyers.Queries.GetFavoriteLawyers;
using Wakiliy.Domain.Constants;

namespace Wakiliy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Client)]
public class FavoritesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all favorite lawyers for the current user
    /// </summary>
    /// <response code="200">List of favorite lawyers returned</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<LawyerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavoriteLawyers(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFavoriteLawyersQuery(User.GetUserId()), cancellationToken);
        return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
    }

    /// <summary>
    /// Add a lawyer to favorites
    /// </summary>
    /// <param name="lawyerId">Lawyer ID to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Lawyer added to favorites</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Lawyer not found</response>
    /// <response code="409">Lawyer already in favorites</response>
    [HttpPost("{lawyerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddFavorite(string lawyerId, CancellationToken cancellationToken)
    {
        var command = new AddFavoriteLawyerCommand { UserId = User.GetUserId(), LawyerId = lawyerId };
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? result.ToSuccess("Lawyer added to favorites successfully") : result.ToProblem();
    }

    /// <summary>
    /// Remove a lawyer from favorites
    /// </summary>
    /// <param name="lawyerId">Lawyer ID to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Lawyer removed from favorites</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Lawyer not in favorites</response>
    [HttpDelete("{lawyerId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveFavorite(string lawyerId, CancellationToken cancellationToken)
    {
        var command = new RemoveFavoriteLawyerCommand { UserId = User.GetUserId(), LawyerId = lawyerId };
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
