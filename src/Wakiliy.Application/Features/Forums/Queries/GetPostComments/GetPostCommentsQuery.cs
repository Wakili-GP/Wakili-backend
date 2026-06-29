using MediatR;
using System.Collections.Generic;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Queries.GetPostComments;

public record GetPostCommentsQuery(
    string PostId,
    string? CurrentUserId = null
) : IRequest<Result<List<ForumCommentResponse>>>;
