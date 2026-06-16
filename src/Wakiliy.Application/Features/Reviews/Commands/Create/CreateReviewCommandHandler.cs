using Mapster;
using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Commands.Create;

public class CreateReviewCommandHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateReviewCommand, Result<ReviewResponseDto>>
{
    public async Task<Result<ReviewResponseDto>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Failure<ReviewResponseDto>(ReviewErrors.AppointmentNotFound);

        if (appointment.Status != AppointmentStatus.Completed)
            return Result.Failure<ReviewResponseDto>(ReviewErrors.AppointmentNotCompleted);

        var reviewExists = await unitOfWork.Reviews.ExistsByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        if (reviewExists)
            return Result.Failure<ReviewResponseDto>(ReviewErrors.ReviewAlreadyExists);

        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            LawyerId = appointment.LawyerId,
            AppointmentId = request.AppointmentId,
            Rating = request.LawyerReview.Rating,
            Comment = request.LawyerReview.Comment,
            CreatedAt = DateTime.UtcNow,
            AiAnalysis = new AiAnalysis
            {
                IsFlagged = request.LawyerReview.AiReview.IsFlagged,
                Confidence = request.LawyerReview.AiReview.Confidence,
                Summary = request.LawyerReview.AiReview.Summary
            }
        };

        await unitOfWork.Reviews.AddAsync(review, cancellationToken);

        var isFirstReview = await unitOfWork.SystemReviews.IsFirstReviewForUserAsync(request.UserId, cancellationToken);
        if (isFirstReview && request.SystemReview is not null)
        {
            var systemReview = new SystemReview
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Rating = request.SystemReview.Rating,
                Comment = request.SystemReview.Comment,
                CreatedAt = DateTime.UtcNow,
                AiAnalysis = new AiAnalysis
                {
                    IsFlagged = request.SystemReview.AiReview.IsFlagged,
                    Confidence = request.SystemReview.AiReview.Confidence,
                    Summary = request.SystemReview.AiReview.Summary
                }
            };

            await unitOfWork.SystemReviews.AddAsync(systemReview, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(review.Adapt<ReviewResponseDto>());
    }
}
