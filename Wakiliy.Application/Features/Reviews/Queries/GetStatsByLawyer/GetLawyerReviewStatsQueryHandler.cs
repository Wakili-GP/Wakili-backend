using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetStatsByLawyer;

public class GetLawyerReviewStatsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetLawyerReviewStatsQuery, Result<LawyerReviewStatsDto>>
{
    public async Task<Result<LawyerReviewStatsDto>> Handle(GetLawyerReviewStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await unitOfWork.Reviews.GetLawyerReviewStatsAsync(request.LawyerId, cancellationToken);

        var result = new LawyerReviewStatsDto
        {
            AverageRating = Math.Round(stats.AverageRating, 2),
            TotalReviews = stats.TotalReviews,
            StarCounts = new Dictionary<int, int>
            {
                [1] = stats.OneStarCount,
                [2] = stats.TwoStarCount,
                [3] = stats.ThreeStarCount,
                [4] = stats.FourStarCount,
                [5] = stats.FiveStarCount
            }
        };

        return Result.Success(result);
    }
}
