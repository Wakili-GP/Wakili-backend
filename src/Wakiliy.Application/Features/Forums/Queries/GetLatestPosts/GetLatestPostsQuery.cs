using MediatR;
using System.Collections.Generic;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Queries.GetLatestPosts;

public record GetLatestPostsQuery(
    int Limit = 6
) : IRequest<Result<List<ForumPostResponse>>>;
