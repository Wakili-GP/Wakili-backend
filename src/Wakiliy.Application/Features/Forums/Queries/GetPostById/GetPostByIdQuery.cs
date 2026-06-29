using MediatR;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Queries.GetPostById;

public record GetPostByIdQuery(
    string PostId,
    string? CurrentUserId = null
) : IRequest<Result<ForumPostResponse>>;
