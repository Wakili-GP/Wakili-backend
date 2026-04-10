using Mapster;
using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetByLawyer;

public class GetReviewsByLawyerIdQueryHandler(IReviewRepository reviewRepository)
    : IRequestHandler<GetReviewsByLawyerIdQuery, Result<List<ReviewResponseDto>>>
{
    public async Task<Result<List<ReviewResponseDto>>> Handle(GetReviewsByLawyerIdQuery request, CancellationToken cancellationToken)
    {
        var reviews = await reviewRepository.GetByLawyerIdAsync(request.LawyerId, cancellationToken);
        var result = reviews.Adapt<List<ReviewResponseDto>>();
        return Result.Success(result);
    }
}
