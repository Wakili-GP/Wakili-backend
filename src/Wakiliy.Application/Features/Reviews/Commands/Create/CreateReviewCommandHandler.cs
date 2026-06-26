using Hangfire;
using Mapster;
using MediatR;
using Wakiliy.Application.Features.Reviews.DTOs;
using Wakiliy.Application.Features.Reviews.Jobs;
using Wakiliy.Application.Interfaces.Services;
using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Errors;
using Wakiliy.Domain.Repositories;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Reviews.Commands.Create;

public class CreateReviewCommandHandler(
    IUnitOfWork unitOfWork,
    INotificationService notificationService,
    IBackgroundJobClient backgroundJobClient)
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
            AiStatus = ReviewAiStatus.Pending,
            Visibility = ReviewVisibility.Pending
        };

        await unitOfWork.Reviews.AddAsync(review, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Enqueue background job for AI review moderation
        backgroundJobClient.Enqueue<ReviewModerationJob>(x => x.ProcessReviewAsync(review.Id));

        await notificationService.SendNotificationAsync(
            userId: appointment.LawyerId,
            title: "تقييم جديد",
            message: "حصلت على تقييم جديد من أحد عملائك.",
            type: NotificationType.NewReview,
            referenceId: review.Id.ToString(),
            cancellationToken: cancellationToken);

        return Result.Success(review.Adapt<ReviewResponseDto>());
    }
}
