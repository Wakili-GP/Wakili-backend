using MediatR;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Queries.GetApprovedPosts;

public record GetApprovedPostsQuery(
    string? Keyword,
    int? SpecializationId,
    string SortBy,
    int Page,
    int Limit,
    string? CurrentUserId = null
) : IRequest<Result<PaginatedResult<ForumPostResponse>>>;
