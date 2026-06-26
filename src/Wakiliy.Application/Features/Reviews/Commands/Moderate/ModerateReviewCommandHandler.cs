using Hangfire;
using MediatR;
using Wakiliy.Domain.Responses;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Repositories;
using Wakiliy.Application.BackgroundJobs;

namespace Wakiliy.Application.Features.Reviews.Commands.Moderate;

public class ModerateReviewCommandHandler : IRequestHandler<ModerateReviewCommand, Result>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public ModerateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IBackgroundJobClient backgroundJobClient)
    {
        _reviewRepository = reviewRepository;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<Result> Handle(ModerateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review == null)
        {
            return Result.Failure(new Error("ReviewNotFound", "Review not found.", 404));
        }

        switch (request.Action)
        {
            case ReviewModerationAction.RetryModeration:
                review.AiStatus = ReviewAiStatus.Pending;
                review.Visibility = ReviewVisibility.Pending;
                await _reviewRepository.UpdateAsync(review, cancellationToken);
                
                _backgroundJobClient.Enqueue<ReviewModerationJob>(j => j.ProcessReviewAsync(review.Id));
                break;
                
            case ReviewModerationAction.Approve:
                review.Visibility = ReviewVisibility.Visible;
                await _reviewRepository.UpdateAsync(review, cancellationToken);
                break;
                
            case ReviewModerationAction.Hide:
                review.Visibility = ReviewVisibility.Hidden;
                await _reviewRepository.UpdateAsync(review, cancellationToken);
                break;
                
            default:
                return Result.Failure(new Error("InvalidAction", "Invalid moderation action.", 400));
        }

        return Result.Success();
    }
}
