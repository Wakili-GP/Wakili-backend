using Mapster;
using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetAllSystemReviews;

public class GetAllSystemReviewsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllSystemReviewsQuery, Result<List<SystemReviewResponseDto>>>
{
    public async Task<Result<List<SystemReviewResponseDto>>> Handle(GetAllSystemReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await unitOfWork.SystemReviews.GetAllAsync(cancellationToken);
        var result = reviews.Adapt<List<SystemReviewResponseDto>>();
        return Result.Success(result);
    }
}
