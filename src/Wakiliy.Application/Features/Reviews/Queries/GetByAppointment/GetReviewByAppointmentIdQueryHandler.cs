using Mapster;
using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Queries.GetByAppointment;

public class GetReviewByAppointmentIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetReviewByAppointmentIdQuery, Result<ReviewResponseDto>>
{
    public async Task<Result<ReviewResponseDto>> Handle(GetReviewByAppointmentIdQuery request, CancellationToken cancellationToken)
    {
        var review = await unitOfWork.Reviews.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        if (review == null)
        {
            return Result.Failure<ReviewResponseDto>(new Error("Review.NotFound", "Review not found", 404));
        }

        return Result.Success(review.Adapt<ReviewResponseDto>());
    }
}
