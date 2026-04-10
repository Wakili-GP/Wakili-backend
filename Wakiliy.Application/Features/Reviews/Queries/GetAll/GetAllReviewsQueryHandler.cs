using Mapster;
using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetAll;

public class GetAllReviewsQueryHandler(IReviewRepository reviewRepository)
    : IRequestHandler<GetAllReviewsQuery, Result<List<ReviewResponseDto>>>
{
    public async Task<Result<List<ReviewResponseDto>>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await reviewRepository.GetAllAsync(cancellationToken);
        var result = reviews.Adapt<List<ReviewResponseDto>>();
        return Result.Success(result);
    }
}
