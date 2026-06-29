using MediatR;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Queries.GetAllPostsForAdmin;

public record GetAllPostsForAdminQuery(
    string? Keyword,
    int? SpecializationId,
    string SortBy,
    int Page,
    int Limit
) : IRequest<Result<PaginatedResult<ForumPostResponse>>>;
