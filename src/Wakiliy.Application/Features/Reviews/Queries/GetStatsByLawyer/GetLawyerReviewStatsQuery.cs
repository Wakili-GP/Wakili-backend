using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetStatsByLawyer;

public class GetLawyerReviewStatsQuery : IRequest<Result<LawyerReviewStatsDto>>
{
    public string LawyerId { get; set; } = string.Empty;
}
