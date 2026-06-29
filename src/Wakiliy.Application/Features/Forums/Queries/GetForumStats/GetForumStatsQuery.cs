using MediatR;
using Wakiliy.Application.Features.Forums.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Forums.Queries.GetForumStats;

public record GetForumStatsQuery() : IRequest<Result<ForumStatsResponse>>;
